const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

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

  // Target paths
  const tempFile = path.resolve(__dirname, 'temp_db_schema.md');
  const targetReadme = path.resolve(readmePath);

  if (!fs.existsSync(targetReadme)) {
    console.error(`Target README not found: ${targetReadme}`);
    process.exit(1);
  }

  try {
    console.log(`Generating database schema using pg-mermaid...`);
    // Run npx pg-mermaid programmatically
    // We pass the PGPASSWORD environment variable so pg-mermaid can authenticate silently without prompt
    execSync(`npx -y pg-mermaid@0.2.1 -h localhost -p ${port} -U postgres -d ${dbName} --output-path "${tempFile}"`, {
      env: { ...process.env, PGPASSWORD: password },
      stdio: 'inherit'
    });

    if (!fs.existsSync(tempFile)) {
      throw new Error("pg-mermaid did not generate the output schema file.");
    }

    let generatedMarkdown = fs.readFileSync(tempFile, 'utf-8');
    
    // Clean up or format the generated markdown to exclude the __EFMigrationsHistory table if present
    // It's cleaner to remove the EFMigrationsHistory from the Mermaid ERD and the Indexes
    generatedMarkdown = generatedMarkdown.replace(/\s*__EFMigrationsHistory\s*\{[^}]*\}/g, '');
    generatedMarkdown = generatedMarkdown.replace(/### `__EFMigrationsHistory`[\s\S]*?(?=### `|$)/g, '');

    // In a microservice README, pg-mermaid's output is perfect. Let's wrap it nicely
    let formattedOutput = 'The primary tables in this microservice:\n\n';
    
    // Extract table names to build a neat index description table
    const tableRegex = /(\w+)\s*\{/g;
    let match;
    const tables = new Set();
    while ((match = tableRegex.exec(generatedMarkdown)) !== null) {
      if (match[1] && match[1] !== 'erDiagram') {
        tables.add(match[1]);
      }
    }

    formattedOutput += '| Table Name | Description |\n';
    formattedOutput += '|------------|-------------|\n';
    for (const tableName of Array.from(tables).sort()) {
      const formattedName = tableName.split('_').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ');
      formattedOutput += `| \`${tableName}\` | Core metadata and storage for ${formattedName}. |\n`;
    }

    formattedOutput += '\n### Entity Relationship Diagram (ERD)\n';
    
    // Extract the Mermaid block
    const mermaidMatch = generatedMarkdown.match(/```mermaid[\s\S]*?```/);
    if (mermaidMatch) {
      formattedOutput += mermaidMatch[0] + '\n\n';
    }

    // Extract the Indexes block
    const indexesMatch = generatedMarkdown.match(/## Indexes[\s\S]*/);
    if (indexesMatch) {
      formattedOutput += indexesMatch[0] + '\n';
    }

    // Inject into target README.md
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
        newLines.push(formattedOutput.trim());
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
      newLines.push(formattedOutput.trim());
      newLines.push('');
    }

    fs.writeFileSync(targetReadme, newLines.join('\n'), 'utf-8');
    console.log(`Successfully generated and updated database schema in ${targetReadme}!`);

  } catch (err) {
    console.error('Error generating schema:', err.message);
  } finally {
    // Clean up temp file
    if (fs.existsSync(tempFile)) {
      fs.unlinkSync(tempFile);
    }
  }
}

main();
