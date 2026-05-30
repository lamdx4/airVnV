const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');
const { Client } = require('pg');

function getPostgresInfo() {
  let port = 5435;
  let password = 'postgres';
  let containerName = '';

  try {
    const output = execSync('docker ps --filter "name=postgres" --format "{{.Names}} {{.Ports}}"').toString().trim();
    const lines = output.split('\n');
    for (const line of lines) {
      const parts = line.split(' ');
      const name = parts[0];
      const ports = parts.slice(1).join(' ');
      
      const match = ports.match(/(?:127\.0\.0\.1|0\.0\.0\.0|localhost|\[::1\]):(\d+)->5432/);
      if (match && match[1]) {
        port = parseInt(match[1], 10);
        containerName = name;
        break;
      }
    }
  } catch (e) {
    console.warn("Could not detect dynamically mapped Postgres port from Docker. Using fallback port 5435.");
  }

  if (containerName) {
    try {
      const inspect = execSync(`docker inspect ${containerName}`).toString().trim();
      const data = JSON.parse(inspect);
      const env = data[0]?.Config?.Env || [];
      for (const item of env) {
        if (item.startsWith('POSTGRES_PASSWORD=')) {
          password = item.split('=')[1];
          break;
        }
      }
    } catch (e) {
      console.warn("Could not inspect Docker container for Postgres password. Using fallback password.");
    }
  }

  return { port, password };
}

async function main() {
  const args = process.argv.slice(2);
  if (args.length < 2) {
    console.error('Usage: node generate-db-schema-from-db.js <dbname> <path-to-readme>');
    process.exit(1);
  }

  const [dbName, readmePath] = args;
  const { port, password } = getPostgresInfo();

  console.log(`Connecting to database "${dbName}" on localhost:${port}...`);

  const client = new Client({
    host: 'localhost',
    port: port,
    database: dbName,
    user: 'postgres',
    password: password,
  });

  try {
    await client.connect();
  } catch (err) {
    console.error(`Failed to connect to database "${dbName}":`, err.message);
    process.exit(1);
  }

  try {
    // 1. Fetch tables and columns
    const columnsQuery = `
      SELECT 
        table_name,
        column_name,
        data_type,
        is_nullable,
        character_maximum_length
      FROM 
        information_schema.columns
      WHERE 
        table_schema = 'public'
        AND table_name NOT LIKE '__EFMigrationsHistory'
      ORDER BY 
        table_name, ordinal_position;
    `;
    const columnsResult = await client.query(columnsQuery);

    // 2. Fetch primary keys
    const pKeysQuery = `
      SELECT 
        kcu.table_name,
        kcu.column_name
      FROM 
        information_schema.table_constraints tc
        JOIN information_schema.key_column_usage kcu 
          ON tc.constraint_name = kcu.constraint_name
          AND tc.table_schema = kcu.table_schema
      WHERE 
        tc.constraint_type = 'PRIMARY KEY'
        AND tc.table_schema = 'public';
    `;
    const pKeysResult = await client.query(pKeysQuery);
    const pKeysSet = new Set(pKeysResult.rows.map(r => `${r.table_name}.${r.column_name}`));

    // 3. Fetch foreign keys and relationships
    const fKeysQuery = `
      SELECT
        tc.table_name AS from_table, 
        kcu.column_name AS from_column, 
        ccu.table_name AS to_table,
        ccu.column_name AS to_column
      FROM 
        information_schema.table_constraints AS tc 
        JOIN information_schema.key_column_usage AS kcu
          ON tc.constraint_name = kcu.constraint_name
          AND tc.table_schema = kcu.table_schema
        JOIN information_schema.constraint_column_usage AS ccu
          ON ccu.constraint_name = tc.constraint_name
          AND ccu.table_schema = tc.table_schema
      WHERE 
        tc.constraint_type = 'FOREIGN KEY'
        AND tc.table_schema = 'public';
    `;
    const fKeysResult = await client.query(fKeysQuery);

    // Group columns by table
    const tables = {};
    for (const row of columnsResult.rows) {
      if (!tables[row.table_name]) {
        tables[row.table_name] = [];
      }
      tables[row.table_name].push(row);
    }

    if (Object.keys(tables).length === 0) {
      console.warn(`No tables found in public schema for database "${dbName}".`);
      process.exit(0);
    }

    // Generate Markdown Tables
    let markdownOutput = 'The primary tables in this microservice:\n\n';
    markdownOutput += '| Table Name | Description |\n';
    markdownOutput += '|------------|-------------|\n';
    for (const tableName of Object.keys(tables)) {
      const formattedName = tableName.split('_').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ');
      markdownOutput += `| \`${tableName}\` | Core metadata and storage for ${formattedName}. |\n`;
    }

    markdownOutput += '\n### Entity Relationship Diagram (ERD)\n';
    markdownOutput += '```mermaid\nerDiagram\n';

    // Generate Mermaid properties
    for (const [tableName, cols] of Object.entries(tables)) {
      markdownOutput += `    ${tableName.toUpperCase()} {\n`;
      for (const col of cols) {
        const isPk = pKeysSet.has(`${tableName}.${col.column_name}`);
        const isFk = fKeysResult.rows.some(fk => fk.from_table === tableName && fk.from_column === col.column_name);
        
        let type = col.data_type;
        if (col.character_maximum_length) {
          type += `(${col.character_maximum_length})`;
        }
        // Simplify common types for cleaner diagram
        type = type.replace('character varying', 'varchar');
        type = type.replace('timestamp with time zone', 'timestamptz');
        
        let keyFlag = '';
        if (isPk && isFk) keyFlag = 'PK,FK';
        else if (isPk) keyFlag = 'PK';
        else if (isFk) keyFlag = 'FK';

        markdownOutput += `        ${type.replace(/\s+/g, '_')} ${col.column_name} ${keyFlag}\n`;
      }
      markdownOutput += '    }\n';
    }

    // Generate Mermaid relationships
    markdownOutput += '\n';
    const drawnRelations = new Set();
    for (const fk of fKeysResult.rows) {
      const relationKey = `${fk.from_table}-${fk.to_table}`;
      if (!drawnRelations.has(relationKey)) {
        markdownOutput += `    ${fk.to_table.toUpperCase()} ||--o{ ${fk.from_table.toUpperCase()} : "has"\n`;
        drawnRelations.add(relationKey);
      }
    }

    markdownOutput += '```\n';

    // Inject into target README.md
    const targetReadme = path.resolve(readmePath);
    if (!fs.existsSync(targetReadme)) {
      console.error(`Target README not found: ${targetReadme}`);
      process.exit(1);
    }

    let readmeContent = fs.readFileSync(targetReadme, 'utf-8');
    const startMarkerRegex = /##.*Database Schema/i;
    
    const lines = readmeContent.split('\n');
    const newLines = [];
    let inSchemaSection = false;
    let schemaInserted = false;

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];
      
      if (startMarkerRegex.test(line)) {
        newLines.push(line);
        newLines.push('');
        newLines.push(markdownOutput.trim());
        newLines.push('');
        inSchemaSection = true;
        schemaInserted = true;
        continue;
      }

      if (inSchemaSection) {
        if (line.startsWith('# ') || line.startsWith('## ')) {
          inSchemaSection = false;
          newLines.push(line);
        }
      } else {
        newLines.push(line);
      }
    }

    if (!schemaInserted) {
      console.warn(`Could not find '## Database Schema' section in ${readmePath}. Appending to end.`);
      newLines.push('\n## 🗄️ Database Schema\n');
      newLines.push(markdownOutput.trim());
      newLines.push('');
    }

    fs.writeFileSync(targetReadme, newLines.join('\n'), 'utf-8');
    console.log(`Successfully generated and updated database schema in ${targetReadme}!`);

  } catch (err) {
    console.error('Error querying database:', err.message);
  } finally {
    await client.end();
  }
}

main();
