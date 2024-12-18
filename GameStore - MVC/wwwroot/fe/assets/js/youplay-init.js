/*!-----------------------------------------------------------------
Name: Youplay - Game Template based on Bootstrap
Version: 3.3.0
Author: nK
Website: https://nkdev.info/
Purchase: https://themeforest.net/item/youplay-game-template-based-on-bootstrap/11306207?ref=_nK
Support: https://nk.ticksy.com/
License: You must have a valid license purchased only from ThemeForest (the above link) in order to legally use the theme for your project.
Copyright 2018.
-------------------------------------------------------------------*/(function(modules){var installedModules={};function __webpack_require__(moduleId){if(installedModules[moduleId]){return installedModules[moduleId].exports;}
var module=installedModules[moduleId]={i:moduleId,l:false,exports:{}};modules[moduleId].call(module.exports,module,module.exports,__webpack_require__);module.l=true;return module.exports;}
__webpack_require__.m=modules;__webpack_require__.c=installedModules;__webpack_require__.d=function(exports,name,getter){if(!__webpack_require__.o(exports,name)){Object.defineProperty(exports,name,{configurable:false,enumerable:true,get:getter});}};__webpack_require__.n=function(module){var getter=module&&module.__esModule?function getDefault(){return module['default'];}:function getModuleExports(){return module;};__webpack_require__.d(getter,'a',getter);return getter;};__webpack_require__.o=function(object,property){return Object.prototype.hasOwnProperty.call(object,property);};__webpack_require__.p="";return __webpack_require__(__webpack_require__.s=1);})
([(function(module,exports,__webpack_require__){"use strict";Object.defineProperty(exports,"__esModule",{value:true});var options={parallax:true,navbarSmall:false,fadeBetweenPages:true,php:{twitter:'./php/twitter/tweet.php',instagram:'./php/instagram/instagram.php'}};exports.options=options;}),(function(module,exports,__webpack_require__){module.exports=__webpack_require__(2);}),(function(module,exports,__webpack_require__){"use strict";var _options=__webpack_require__(0);if(typeof window.youplay!=='undefined'){window.youplay.init(_options.options);}})]);