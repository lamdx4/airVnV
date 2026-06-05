const fs = require('fs');
const path = require('path');

async function main() {
  const args = process.argv.slice(2);
  if (args.length < 2) {
    console.error('Usage: node generate-endpoints.js <swagger-url-or-file> <path-to-readme>');
    process.exit(1);
  }

  const [swaggerSource, readmePath] = args;
  let swaggerData;

  try {
    if (swaggerSource.startsWith('http://') || swaggerSource.startsWith('https://')) {
      console.log(`Fetching swagger from ${swaggerSource}...`);
      const response = await fetch(swaggerSource);
      if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
      swaggerData = await response.json();
    } else {
      console.log(`Reading swagger from local file ${swaggerSource}...`);
      const fileContent = fs.readFileSync(path.resolve(swaggerSource), 'utf-8');
      swaggerData = JSON.parse(fileContent);
    }
  } catch (error) {
    console.error('Failed to load Swagger JSON:', error.message);
    process.exit(1);
  }

  if (!swaggerData.paths) {
    console.error('Invalid Swagger JSON: missing "paths" object.');
    process.exit(1);
  }

  // Generate markdown table
  let tableMarkdown = '| Method | Path | Description |\n';
  tableMarkdown += '|--------|------|-------------|\n';

  for (const [routePath, methods] of Object.entries(swaggerData.paths)) {
    for (const [method, details] of Object.entries(methods)) {
      // Swagger contains some non-http-method keys sometimes (like parameters)
      if (['get', 'post', 'put', 'patch', 'delete'].includes(method.toLowerCase())) {
        const upperMethod = method.toUpperCase();
        let description = details.summary || details.description || '';
        // Escape pipe characters in description to not break markdown tables
        description = description.replace(/\|/g, '\\|');
        // Clean up newlines
        description = description.replace(/\n/g, ' ');
        
        tableMarkdown += `| **${upperMethod}** | \`${routePath}\` | ${description} |\n`;
      }
    }
  }

  // Read and update README.md
  const targetReadme = path.resolve(readmePath);
  if (!fs.existsSync(targetReadme)) {
    console.error(`Target README not found: ${targetReadme}`);
    process.exit(1);
  }

  let readmeContent = fs.readFileSync(targetReadme, 'utf-8');
  
  // Regex to detect "## API Endpoints" heading (case insensitive, allows emojis)
  const startMarkerRegex = /##.*API Endpoints/i;
  
  const lines = readmeContent.split('\n');
  const newLines = [];
  let inApiSection = false;
  let tableInserted = false;

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    
    if (startMarkerRegex.test(line)) {
      newLines.push(line);
      newLines.push('');
      newLines.push(tableMarkdown.trim());
      newLines.push('');
      inApiSection = true;
      tableInserted = true;
      continue;
    }

    if (inApiSection) {
      // If we encounter the next H2 or H1, we exit the API section
      if (line.startsWith('# ') || line.startsWith('## ')) {
        inApiSection = false;
        newLines.push(line);
      }
      // Otherwise, skip the old content inside API section (we are overwriting it)
    } else {
      newLines.push(line);
    }
  }

  if (!tableInserted) {
    console.warn(`Warning: Could not find '## API Endpoints' section in ${readmePath}. Appending to end.`);
    newLines.push('\n## 🔌 API Endpoints\n');
    newLines.push(tableMarkdown.trim());
    newLines.push('');
  }

  fs.writeFileSync(targetReadme, newLines.join('\n'), 'utf-8');
  console.log(`Successfully updated ${targetReadme} with API endpoints!`);
}

main();
