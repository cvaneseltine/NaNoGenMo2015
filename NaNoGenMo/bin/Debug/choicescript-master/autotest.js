/*
 * Copyright 2010 by Dan Fabulich.
 * 
 * Dan Fabulich licenses this file to you under the
 * ChoiceScript License, Version 1.0 (the "License"); you may
 * not use this file except in compliance with the License. 
 * You may obtain a copy of the License at
 * 
 *  http://www.choiceofgames.com/LICENSE-1.0.txt
 * 
 * See the License for the specific language governing
 * permissions and limitations under the License.
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
 * either express or implied.
 */

 // autotest.js mygame [sceneName1] [sceneName2] [sceneName3]
var list;
var gameName;
if (typeof java == "undefined") {
  list = process.argv;
  list.shift();
  list.shift();
  gameName = list.shift();
  if (!gameName) gameName = "mygame";
  fs = require('fs');
  vm = require('vm');
  path = require('path');
  load = function(file) {
    vm.runInThisContext(fs.readFileSync(file), file);
  };
  load("web/scene.js");
  load("web/navigator.js");
  load("web/util.js");
  load("headless.js");
  load("web/"+gameName+"/"+"mygame.js");
  load("editor/embeddable-autotester.js");
  print = function print(str) {
    console.log(str);
  };
} else {
  list = arguments;
  gameName = list.shift();
  if (!gameName) gameName = "mygame";
  load("web/scene.js");
  load("web/navigator.js");
  load("web/util.js");
  load("headless.js");
  load("web/"+gameName+"/"+"mygame.js");
  load("editor/embeddable-autotester.js");
  if (typeof(console) == "undefined") console = {log: print};
}

nav.setStartingStatsClone(stats);

var sceneList = [];

function debughelp() {
    debugger;
}
var fullGame = false;
if (!list.length || (list.length == 1 && !list[0])) {
  fullGame = true;
  list = [];
  for (var i = 0; i < nav._sceneList.length; i++) {
    addFile(nav._sceneList[i]+".txt");
  }
  if (fileExists("web/"+gameName+"/scenes/choicescript_stats.txt")) {
    list.push("choicescript_stats.txt");
  }
  if (fileExists("web/"+gameName+"/scenes/choicescript_screenshots.txt")) {
    list.push("choicescript_screenshots.txt");
  }
  if (fileExists("web/"+gameName+"/scenes/choicescript_upgrade.txt")) {
    list.push("choicescript_upgrade.txt");
  }
} else {
  for (var i = list.length - 1; i >= 0; i--) {
    list[i] += ".txt";
  }
}

function addFile(name) {
  for (var i = 0; i < list.length; i++) {
    if (list[i] == name) return;
  }
  list.push(name);
}

var uncoveredScenes = [];
var uncovered;

var sceneFileSets = {};
verifyFileName = function verifyFileName(dir, name) {
  var filePath = "web/"+gameName+"/"+dir+"/"+name;
  if (!fileExists(filePath)) throw new Error("File does not exist: " + name);
  var canonicalName, fileName, i;
  if (isRhino) {
    var file = new java.io.File(filePath);
    fileName = file.getName();
    canonicalName = file.getCanonicalFile().getName();
    if (fileName != canonicalName) throw new Error("Incorrect capitalization/canonicalization; the file is called " + canonicalName + " but you requested " + name);
  } else {
    if (!sceneFileSets[dir]) {
      sceneFileSets[dir] = {};
      var sceneFiles = fs.readdirSync("web/"+gameName+"/"+dir);
      for (i = sceneFiles.length - 1; i >= 0; i--) {
        sceneFileSets[dir][sceneFiles[i]] = 1;
      }
    }
    if (!sceneFileSets[dir][name]) {
      for (var sceneFile in sceneFileSets[dir]) {
        if (sceneFile.toLowerCase() == name.toLowerCase()) {
          throw new Error("Incorrect capitalization/canonicalization; the file is called " + sceneFile + " but you requested " + name);
        }
      }
      throw new Error("Incorrect capitalization/canonicalization? you requested " + name + " but that file doesn't exist");
    }
  }
};

Scene.prototype.verifySceneFile = function commandLineVerifySceneFile(sceneName) {
  var fileName = sceneName +".txt";
  try {
    verifyFileName("scenes", fileName);
    if (fullGame) {
      addFile(fileName);
    }
  } catch (e) {
    throw new Error(this.lineMsg() + e.message);
  }
};

Scene.prototype.verifyImage = function commandLineVerifyImage(name) {
  try {
    verifyFileName(".", name);
  } catch (e) {
    throw new Error(this.lineMsg() + e.message);
  }
};

// test startup scene first, to run *create commands
if (list[0] != nav.getStartupScene()+".txt") list.unshift(nav.getStartupScene()+".txt");

var titleIncluded, authorIncluded;
var gotoSceneLabels = {};

// enumerate scenes; gather visited labels

if (fullGame) {
  (function(){
    function parseSceneList(lines, lineNum) {
      var nextIndent = null;
      var scenes = [];
      var purchases = {};
      var line;
      while(typeof (line = lines[++lineNum]) != "undefined") {
          if (!line.trim()) continue;

          var indent = /^(\s*)/.exec(line)[1].length;
          if (nextIndent === null || nextIndent === undefined) {
              // initialize nextIndent with whatever indentation the line turns out to be
              // ...unless it's not indented at all
              if (indent === 0) throw new Error("invalid scene_list indent, expected at least one row");
              this.indent = nextIndent = indent;
          }
          if (indent === 0) break;
          if (indent != this.indent) {
              // all scenes are supposed to be at the same indentation level
              throw new Error("invalid scene_list indent, expected "+this.indent+", was " + indent);
          }

          line = line.trim();
          var purchaseMatch = /^\$(\w*)\s+(.*)/.exec(line);
          if (purchaseMatch) {
            line = purchaseMatch[2];
            var product = purchaseMatch[1].trim() || "adfree";
            purchases[line] = product;
          }
          if (!scenes.length && "startup" != line) scenes.push("startup");
          scenes.push(line);
      }
      return {scenes:scenes, purchases:purchases, lineNum:lineNum-1};
    }


    for (var i = 0; i < list.length; i++) {
      try {
        var fileName = list[i];
        var sceneName = fileName.replace(/\.txt$/, "");
        verifyFileName("scenes", fileName);
        var sceneText = slurpFile("web/"+gameName+"/scenes/"+fileName, true /*throwOnError*/);
        var sceneLines = sceneText.split("\n");
        for (var j = 0; j < sceneLines.length; j++) {
          var line = sceneLines[j];
          var words;
          var result = /^\s*\*(\w+)(.*)/.exec(line);
          if (!result) continue;
          var command = result[1].toLowerCase();
          var data = trim(result[2]);
          if (command == "goto_scene" || command == "gosub_scene") {
            if (data === null) data = "";
            if (/[\[\{]/.test(data)) {
              // print warning?
            } else {
              words = data.split(/ /);
              if (words[0] !== "") {
                addFile(words[0]+".txt");
              }
              if (words.length > 1) {
                if (!gotoSceneLabels[words[0]]) gotoSceneLabels[words[0]] = [];
                gotoSceneLabels[words[0]].push({origin:sceneName, originLine:j, label:words[1]});
              }
            }
          } else if (command == "save_game") {
            if (data !== null) addFile(data+".txt");
          } else if (command == "scene_list" && i === 0) {
            var sceneList = parseSceneList(sceneLines, j);
            j = sceneList.lineNum;
            for (var k = 0; k < sceneList.scenes.length; k++) {
              addFile(sceneList.scenes[k]+".txt");
            }
          }
        }
      } catch (e) {
        print("Error parsing "+sceneName+" line "+(j+1)+ "; we'll flag the error in detail later\n");
        print(e);
      }
    }
  }());
}



(function(){
  for (var i = 0; i < list.length; i++) {
    print(list[i]);
    if (isRhino) java.lang.Thread.sleep(100); // sleep to allow print statements to flush :-(
    try {
      var fileName = list[i];
      var sceneName = fileName.replace(/\.txt$/, "");
      verifyFileName("scenes", fileName);
      var sceneText = slurpFile("web/"+gameName+"/scenes/"+fileName, true /*throwOnError*/);
      if (i === 0) {
        titleIncluded = /\*title /m.test(sceneText);
        authorIncluded = /^\*author /m.test(sceneText);
      }
      uncovered = autotester(sceneText, nav, sceneName, gotoSceneLabels[sceneName])[1];
    } catch (e) {
      print("QUICKTEST FAILED\n");
      print(e);
      if (isRhino) {
        java.lang.System.exit(1);
      } else {
        process.exit(1);
      }
    }
    if (uncovered) {
      uncoveredScenes.push({name:list[i], lines:uncovered});
    }
  }
}());


var allLinesTested = true;
for (var i = 0; i < uncoveredScenes.length; i++) {
  allLinesTested = false;
  var uncoveredScene = uncoveredScenes[i];
  uncoveredScene.lines.push("");
  print(uncoveredScene.lines.join(" UNTESTED " + uncoveredScene.name + "\n"));
}
(function() {
  if (nav.achievementList && nav.achievementList.length) {
    for (var i = 0; i < nav.achievementList.length; i++) {
      var name = nav.achievementList[i];
      if (!nav.achieved[name]) {
        print("UNUSED achievement: " + name);
      }
    }
  }
})();


if (!allLinesTested) print("SOME LINES UNTESTED");
if (!titleIncluded) print("MISSING *TITLE COMMAND");
if (!authorIncluded) print("MISSING *AUTHOR COMMAND");
print("QUICKTEST PASSED");
