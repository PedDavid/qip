import Tool from './tools/Tool'
import Pen from './tools/Pen'
import Eraser from './tools/Eraser'

// todo (simaovii) tirar esta classe do model. por em utils

// this class has the purpose to configure all tools and simplify all the management of the tools.
// DefaultTools can be accessed two ways:
// by calling ToolsConfig.defaultTools -> Tool[]
// by calling ToolsConfig[pen|eraser|move] -> ConfigTool[] with pens
export default class ToolsConfig {
  constructor (defaultToolsRaw) {
    this.defaultTools = defaultToolsRaw
    this._mapTools(defaultToolsRaw)
  }

  // !!! at the moment, this is not working
  getDefaultToolOf = function (toolType: Tool) {
    const idx = this.defaultTools
      .findIndex(defTool => {
        if (defTool.toolType !== undefined) {
          return defTool.toolType.name === toolType
        }
        return false
      })
    return this.defaultTools[idx]
  }

  // private method to map all the defaultTools to a type so it can be possible
  // to do calls like: ToolsConfig[pen] ...
  _mapTools = function (defaultTools) {
    defaultTools.forEach(defTool => {
      this[defTool.type] = new DefaultTool(defTool.type, defTool.icon, defTool.content, defTool.lastValue, defTool.toolType)
    })
  }

  // method to facilitate the change of tool. This way it can be called only with one
  // parameter. It will automatically set the previous tool of each tool type
  updatePrevTool = function (prevTool) {
    if (prevTool instanceof Pen) {
      this.pen.lastValue = prevTool
      this.width.lastValue = prevTool // mantain this here to be easier to update width
    } else if (prevTool instanceof Eraser) {
      this.eraser.lastValue = prevTool
    }
  }
}

// this class represents a DefaultTool.
// With this class, when writting code, it will be simplier to get tools' properties
class DefaultTool {
  constructor (type, icon, content, lastValue, toolType: Tool) {
    this.type = type
    this.icon = icon
    this.content = content
    this.lastValue = lastValue
    this.toolType = toolType
  }
}
