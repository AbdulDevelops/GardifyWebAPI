const glob = require('glob');
const fs = require("fs")

const LICENSES_PATH = "licenses.txt"
const FILE_ENCODING = "UTF-8"

glob("dist/gardify/main.*.js", function (er, files) {
    if(files.length != 1) {
        console.log("Ambiguous files found: ", files)
        return
    }

    const path = files[0]
    const mainJs = fs.readFileSync(path, FILE_ENCODING)
    const licensesTxt = fs.readFileSync(LICENSES_PATH, FILE_ENCODING)

    const result = licensesTxt + "\n" + mainJs
    fs.writeFileSync(path, result, FILE_ENCODING)
})
