﻿var gulp = require("gulp"),
    rimraf = require("rimraf"),
    through = require("through2"),
    os = require('os'),
    PluginError = require('gulp-util').PluginError,
    File = require('vinyl');

var inputGlob = "./ProductGuide/*.cs";
var outputPath = "../../examples-output/";

gulp.task("clean", function (cb) {
  rimraf(outputPath, cb);
});

gulp.task("extract-samples", function () {
    gulp.src(inputGlob)
        .pipe(extractSamples())
        .pipe(gulp.dest(outputPath));
});

function extractSamples() {
  var self = this;

  return through.obj(function (file, enc, cb) {
    if (file.isNull()) {
      return cb(null, file);
    }

    if (file.isStream()) {
      self.emit("error", new PluginError("gulp-extract-samples", "Streams aren't supported."));
      return cb();
    }

    if (file.isBuffer()) {
      var contents = file.contents.toString(enc);

      var extracted = parseFile(contents);

      // check for file without samples
      if (extracted.length === 0) {
        return cb(null, file);
      }

      // create a new Vinyl file for each sample
      extracted.forEach(function (s) {
        this.push(new File({
          path: s.name,
          contents: new Buffer(s.contents)
        }));
      }.bind(this));
      return cb();
    }

    cb(null, file);
  });
}

function parseFile(data) {
  var startToken = "#region";
  var endToken = "#endregion";
  var pos = 0;

  var extracted = [];

  while (pos < data.length) {
    // Find the beginning of a code sample
    var start = data.indexOf(startToken, pos);
    if (start === -1) {
      break;
    }

    var eol = data.indexOf(os.EOL, start);

    var name = data.substring(start + startToken.length, eol).trim();

    start = eol + os.EOL.length;

    var end = data.indexOf(endToken, pos);
    if (end === -1) {
      break;
    }

    var contents = data.substring(start, end);
    contents = trimStart(os.EOL, contents);
    contents = trimEndWhitespace(contents);

    var indentation = findIndentation(contents);
    contents = reduceWhitespace(contents, indentation);

    extracted.push({ name: name, contents: contents });

    pos = end + endToken.length;
  }

  return extracted;
}

// Returns the whitespace amount from the first line
function findIndentation(input) {
  var first = input.split(os.EOL)[0];
  return first.search(/\S|$/);
}

// Removes the specified amount of whitespace from the beginning of each line
function reduceWhitespace(input, normativeWhitespace) {
    return input
        .split(os.EOL)
        .map(function(line) {
            return line.substring(normativeWhitespace);
        })
        .join(os.EOL);
}

function trimStart(stringToRemove, input) {
    var startIndex = 0;
    var len = stringToRemove.length;

    while (input.substr(startIndex, len) === stringToRemove) {
        startIndex += len;
    }

    return input.substr(startIndex);
}

function trimEndWhitespace(input) {
    return input.replace(/\s*$/, "");
}