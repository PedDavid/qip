// @Flow
import Tool from './tools/Tool'
import Pen from './tools/Pen'
import Eraser from './tools/Eraser'

export default class ToolsConfig {
  constructor (defaultToolsRaw) {
    this.defaultTools = defaultToolsRaw
    this._mapTools(defaultToolsRaw)
  }

  getDefaultToolOf = function (toolType: Tool) {
    const idx = this.defaultTools.findIndex(defTool => defTool.toolType instanceof toolType)
    return idx && this.defaultTools[idx]
  }

  _mapTools = function (defaultTools) {
    defaultTools.forEach(defTool => {
      this[defTool.type] = new DefaultTool(defTool.type, defTool.icon, defTool.content, defTool.lastValue, defTool.toolType)
    })
  }
  updatePrevTool = function (prevTool) {
    if (prevTool instanceof Pen) {
      this.pen.lastValue = prevTool
      this.width.lastValue = prevTool // mantain this here to be easier to update width
    } else if (prevTool instanceof Eraser) {
      this.eraser.lastValue = prevTool
    }
  }
}

class DefaultTool {
  constructor (type, icon, content, lastValue, toolType: Tool) {
    this.type = type
    this.icon = icon
    this.content = content
    this.lastValue = lastValue
    this.toolType = toolType
  }
}
