const esbuild = require('esbuild');
const fs = require('fs');
const path = require('path');

// 1. Define where r JS files are
const jsDir = './wwwroot/js';

// 2. Read all files in that directory
try {
    const files = fs.readdirSync(jsDir);

    console.log(`Found ${files.length} files in ${jsDir}...`);

    files.forEach(file => {
        // 3. Filter: Only process .js files that are NOT already minified
        if (path.extname(file) === '.js' && !file.includes('.min.js')) {
            
            const inputPath = path.join(jsDir, file);
            // Create output name: "combo.js" -> "combo.min.js"
            const outputPath = path.join(jsDir, file.replace('.js', '.min.js'));

            console.log(`📦 Minifying: ${file} -> ${path.basename(outputPath)}`);

            // 4. Run esbuild for this specific file
            esbuild.buildSync({
                entryPoints: [inputPath],
                outfile: outputPath,
                minify: true,       // Shrink the code
                bundle: false,      // set to true if you use 'import' statements
                sourcemap: false,   // set to true if you want to debug in browser
            });
        }
    });

    console.log("✅ All files minified successfully!");

} catch (err) {
    console.error("❌ Build failed:", err);
    process.exit(1);
}