/*
 * ATTENTION: The "eval" devtool has been used (maybe by default in mode: "development").
 * This devtool is neither made for production nor for readable output files.
 * It uses "eval()" calls to create a separate source file in the browser devtools.
 * If you are trying to read the output file, select a different devtool (https://webpack.js.org/configuration/devtool/)
 * or disable the default devtool with "devtool: false".
 * If you are looking for production-ready output files, see mode: "production" (https://webpack.js.org/configuration/mode/).
 */
var pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad;
/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./TableSelector/DataService.ts":
/*!**************************************!*\
  !*** ./TableSelector/DataService.ts ***!
  \**************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

eval("{__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   DataService: () => (/* binding */ DataService),\n/* harmony export */   DataServiceMock: () => (/* binding */ DataServiceMock)\n/* harmony export */ });\nvar __awaiter = undefined && undefined.__awaiter || function (thisArg, _arguments, P, generator) {\n  function adopt(value) {\n    return value instanceof P ? value : new P(function (resolve) {\n      resolve(value);\n    });\n  }\n  return new (P || (P = Promise))(function (resolve, reject) {\n    function fulfilled(value) {\n      try {\n        step(generator.next(value));\n      } catch (e) {\n        reject(e);\n      }\n    }\n    function rejected(value) {\n      try {\n        step(generator[\"throw\"](value));\n      } catch (e) {\n        reject(e);\n      }\n    }\n    function step(result) {\n      result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);\n    }\n    step((generator = generator.apply(thisArg, _arguments || [])).next());\n  });\n};\nclass DataService {\n  constructor(webApi) {\n    this.webApi = webApi;\n  }\n  getAvailableTables() {\n    return __awaiter(this, void 0, void 0, function* () {\n      var execute_pg_getunsynchronizedtables_Request = {\n        getMetadata: function getMetadata() {\n          return {\n            boundParameter: null,\n            parameterTypes: {},\n            operationType: 1,\n            operationName: \"pg_getunsynchronizedtables\"\n          };\n        }\n      };\n      var anyWebAPI = this.webApi;\n      try {\n        var result = yield anyWebAPI.execute(execute_pg_getunsynchronizedtables_Request);\n        if (result.ok) {\n          console.log(\"Custom API executed successfully.\");\n          var jsonResponse = yield result.json();\n          if (jsonResponse) {\n            try {\n              console.log(\"Extracting data from response body.\");\n              var jsonString = jsonResponse[\"tables\"];\n              var tables = JSON.parse(jsonString);\n              console.log(\"\".concat(tables.length, \" tables retrieved.\"));\n              return tables;\n            } catch (error) {\n              console.log(\"Error parsing response body.\");\n              throw new Error(\"Failed to parse response body\");\n            }\n          } else {\n            console.log(\"Cannot extract data from response body\");\n            throw new Error(\"Cannot extract data from response body\");\n          }\n        } else {\n          console.log(\"Error executing custom API.\");\n          console.log(result);\n          throw new Error(\"Custom API execution failed with status: \".concat(result.status));\n        }\n      } catch (error) {\n        console.error(\"Unhandled error in getAvailableTables:\", error);\n        return [];\n      }\n    });\n  }\n}\nclass DataServiceMock {\n  getAvailableTables() {\n    return __awaiter(this, void 0, void 0, function* () {\n      return Promise.resolve([{\n        Name: \"Local Account\",\n        SchemaName: \"account_local\"\n      }, {\n        Name: \"Local Contact\",\n        SchemaName: \"contact_local\"\n      }, {\n        Name: \"Local Lead\",\n        SchemaName: \"lead_local\"\n      }]);\n    });\n  }\n}\n\n//# sourceURL=webpack://pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad/./TableSelector/DataService.ts?\n}");

/***/ }),

/***/ "./TableSelector/TableSelectorControl.tsx":
/*!************************************************!*\
  !*** ./TableSelector/TableSelectorControl.tsx ***!
  \************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

eval("{__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   TableSelectorControl: () => (/* binding */ TableSelectorControl)\n/* harmony export */ });\n/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ \"react\");\n/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);\n/* harmony import */ var _fluentui_react_components__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @fluentui/react-components */ \"@fluentui/react-components\");\n/* harmony import */ var _fluentui_react_components__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_fluentui_react_components__WEBPACK_IMPORTED_MODULE_1__);\n\n\n// const data = ['Eugenia', 'Bryan', 'Linda', 'Nancy', 'Lloyd', 'Alice', 'Julia', 'Albert'].map(\n//   item => ({ label: item, value: item })\n// );\n// Static styles object since we can't use hooks in class components\nvar styles = {\n  root: {\n    width: \"100%\",\n    minWidth: \"200px\"\n  }\n};\nclass TableSelectorControl extends react__WEBPACK_IMPORTED_MODULE_0__.Component {\n  constructor(props) {\n    super(props);\n    this.state = {\n      selectedTableName: \"\",\n      tables: []\n    };\n    this.handleComboboxChange = (event, data) => {\n      var selectedValue = data.optionValue;\n      // Update local state\n      this.setState({\n        selectedTableName: selectedValue\n      });\n      // Call the parent's onChange callback\n      if (this.props.onChange) {\n        this.props.onChange(selectedValue !== null && selectedValue !== void 0 ? selectedValue : \"\");\n      }\n    };\n    this.tableSchemaName = props.tableSchemaName; //Selected table schema name\n    this.isDisabled = props.isDisabled;\n    this.theme = props.theme;\n    this.isCanvasApp = props.isCanvasApp;\n    this.dataService = props.dataService;\n    // Initialize state\n    this.state = {\n      selectedTableName: \"\",\n      tables: []\n    };\n  }\n  componentDidMount() {\n    console.log('Component mounted. Loading tables...');\n    this.dataService.getAvailableTables().then(tables => {\n      console.log(tables);\n      var tablesArray = Array.isArray(tables) ? tables : [];\n      this.setState({\n        tables: tables\n      });\n      return tables;\n      //Example of tables data:\n      // [{\"Name\":\"Account\",\"SchemaName\":\"account\"},\n      // {\"Name\":\"Contact\",\"SchemaName\":\"contact\"},\n      // {\"Name\":\"Lead\",\"SchemaName\":\"lead\"}]\n    }).catch(error => {\n      console.error('Error loading tables:', error);\n      this.setState({\n        tables: []\n      });\n    });\n  }\n  render() {\n    var _a, _b, _c, _d;\n    var myTheme = this.isDisabled && this.isCanvasApp === false ? Object.assign(Object.assign({}, this.theme), {\n      colorCompoundBrandStroke: (_a = this.theme) === null || _a === void 0 ? void 0 : _a.colorNeutralStroke1,\n      colorCompoundBrandStrokeHover: (_b = this.theme) === null || _b === void 0 ? void 0 : _b.colorNeutralStroke1Hover,\n      colorCompoundBrandStrokePressed: (_c = this.theme) === null || _c === void 0 ? void 0 : _c.colorNeutralStroke1Pressed,\n      colorCompoundBrandStrokeSelected: (_d = this.theme) === null || _d === void 0 ? void 0 : _d.colorNeutralStroke1Selected\n    }) : this.theme;\n    // Add safety check for tables array\n    var tablesArray = Array.isArray(this.state.tables) ? this.state.tables : [];\n    return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0__.createElement(_fluentui_react_components__WEBPACK_IMPORTED_MODULE_1__.FluentProvider, {\n      theme: myTheme\n    }, !this.isDisabled || this.isCanvasApp === true ? /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0__.createElement(_fluentui_react_components__WEBPACK_IMPORTED_MODULE_1__.Combobox\n    //value={this.name}          \n    , {\n      //value={this.name}          \n      appearance: 'filled-darker',\n      style: styles.root,\n      listbox: {\n        style: styles.root\n      },\n      readOnly: this.isDisabled,\n      disabled: this.isDisabled && this.isCanvasApp === true,\n      placeholder: \"Select an option...\",\n      \"aria-label\": \"Table selector\",\n      onOptionSelect: this.handleComboboxChange\n    }, tablesArray.map(item => (/*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0__.createElement(_fluentui_react_components__WEBPACK_IMPORTED_MODULE_1__.Option, {\n      key: item.SchemaName,\n      value: item.SchemaName,\n      text: \"\".concat(item.SchemaName, \" (\").concat(item.Name, \")\"),\n      style: styles.root\n    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0__.createElement(\"span\", {\n      style: styles.root\n    }, \"\".concat(item.SchemaName, \" (\").concat(item.Name, \")\")))))) : /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0__.createElement(_fluentui_react_components__WEBPACK_IMPORTED_MODULE_1__.Input, {\n      value: this.tableSchemaName,\n      appearance: 'filled-darker',\n      style: styles.root,\n      readOnly: this.isDisabled\n    }));\n  }\n}\n\n//# sourceURL=webpack://pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad/./TableSelector/TableSelectorControl.tsx?\n}");

/***/ }),

/***/ "./TableSelector/index.ts":
/*!********************************!*\
  !*** ./TableSelector/index.ts ***!
  \********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

eval("{__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   TableSelector: () => (/* binding */ TableSelector)\n/* harmony export */ });\n/* harmony import */ var _TableSelectorControl__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./TableSelectorControl */ \"./TableSelector/TableSelectorControl.tsx\");\n/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! react */ \"react\");\n/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_1__);\n/* harmony import */ var _DataService__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./DataService */ \"./TableSelector/DataService.ts\");\n\n\n\nclass TableSelector {\n  /**\n   * Empty constructor.\n   */\n  constructor() {\n    // Empty\n  }\n  /**\n   * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.\n   * Data-set values are not initialized here, use updateView.\n   * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.\n   * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.\n   * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.\n   */\n  init(context, notifyOutputChanged, state) {\n    this.notifyOutputChanged = notifyOutputChanged;\n  }\n  /**\n   * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.\n   * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions\n   * @returns ReactElement root react element for the control\n   */\n  updateView(context) {\n    var _a, _b, _c, _d, _e, _f;\n    var isDisabled = false;\n    this.tableSchemaName = (_c = (_b = (_a = context === null || context === void 0 ? void 0 : context.parameters) === null || _a === void 0 ? void 0 : _a.tableName) === null || _b === void 0 ? void 0 : _b.raw) !== null && _c !== void 0 ? _c : \"\";\n    if (this.tableSchemaName && this.tableSchemaName.length > 0) {\n      isDisabled = true;\n    }\n    var dataService = this.isRunningOnLocalhost(context) ? new _DataService__WEBPACK_IMPORTED_MODULE_2__.DataServiceMock() : new _DataService__WEBPACK_IMPORTED_MODULE_2__.DataService(context.webAPI);\n    var props = {\n      controlContext: context,\n      tableSchemaName: this.tableSchemaName,\n      isDisabled: isDisabled,\n      theme: (_d = context === null || context === void 0 ? void 0 : context.fluentDesignLanguage) === null || _d === void 0 ? void 0 : _d.tokenTheme,\n      isCanvasApp: ((_f = (_e = context === null || context === void 0 ? void 0 : context.parameters) === null || _e === void 0 ? void 0 : _e.isCanvas) === null || _f === void 0 ? void 0 : _f.raw) === \"Yes\",\n      dataService: dataService,\n      onChange: this.onChange.bind(this)\n    };\n    return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_1__.createElement(_TableSelectorControl__WEBPACK_IMPORTED_MODULE_0__.TableSelectorControl, props);\n  }\n  /**\n   * It is called by the framework prior to a control receiving new data.\n   * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as \"bound\" or \"output\"\n   */\n  getOutputs() {\n    return {\n      tableName: this.tableSchemaName\n    };\n  }\n  onChange(value) {\n    this.tableSchemaName = value;\n    this.notifyOutputChanged();\n  }\n  destroy() {\n    // Add code to cleanup control if necessary\n  }\n  isRunningOnLocalhost(context) {\n    try {\n      if (typeof window !== 'undefined' && window.location) {\n        var hostname = window.location.hostname;\n        if (hostname === 'localhost' || hostname === '127.0.0.1' || hostname.includes('localhost')) {\n          return true;\n        }\n      }\n      return false;\n    } catch (error) {\n      console.log('Error detecting environment, defaulting to non-localhost mode:', error);\n      return false;\n    }\n  }\n}\n\n//# sourceURL=webpack://pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad/./TableSelector/index.ts?\n}");

/***/ }),

/***/ "@fluentui/react-components":
/*!************************************!*\
  !*** external "FluentUIReactv940" ***!
  \************************************/
/***/ ((module) => {

module.exports = FluentUIReactv940;

/***/ }),

/***/ "react":
/*!***************************!*\
  !*** external "Reactv16" ***!
  \***************************/
/***/ ((module) => {

module.exports = Reactv16;

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/compat get default export */
/******/ 	(() => {
/******/ 		// getDefaultExport function for compatibility with non-harmony modules
/******/ 		__webpack_require__.n = (module) => {
/******/ 			var getter = module && module.__esModule ?
/******/ 				() => (module['default']) :
/******/ 				() => (module);
/******/ 			__webpack_require__.d(getter, { a: getter });
/******/ 			return getter;
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module can't be inlined because the eval devtool is used.
/******/ 	var __webpack_exports__ = __webpack_require__("./TableSelector/index.ts");
/******/ 	pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad = __webpack_exports__;
/******/ 	
/******/ })()
;
if (window.ComponentFramework && window.ComponentFramework.registerControl) {
	ComponentFramework.registerControl('Pg.DataverseSync.Controls.TableSelector', pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad.TableSelector);
} else {
	var Pg = Pg || {};
	Pg.DataverseSync = Pg.DataverseSync || {};
	Pg.DataverseSync.Controls = Pg.DataverseSync.Controls || {};
	Pg.DataverseSync.Controls.TableSelector = pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad.TableSelector;
	pcf_tools_652ac3f36e1e4bca82eb3c1dc44e6fad = undefined;
}