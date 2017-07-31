export default class Selection {
  constructor (topLeftPoint, bottomRighPoint, canvasContext) {
    this.topLeftPoint = topLeftPoint
    this.canvasContext = canvasContext
    this.width = bottomRighPoint.x - topLeftPoint.x
    this.height = bottomRighPoint.y - topLeftPoint.y

    this._circleProperties = []
    this._updateCircleProperties()
  }

  _updateCircleProperties = function () {
    const {x, y} = this.topLeftPoint
    const width = this.width
    const height = this.height
    this._circleProperties = [
      {x, y, rad: 5, beginA: Math.PI * 0.5, endA: Math.PI * 2, scale: 'tl'}, // top left
      {x: x + width, y, rad: 5, beginA: Math.PI, endA: Math.PI * 2.5, scale: 'tr'}, // top right
      {x, y: y + height, rad: 5, beginA: 0, endA: Math.PI * 1.5, scale: 'bl'}, // bottom left
      {x: x + width, y: y + height, rad: 5, beginA: -Math.PI * 0.5, endA: Math.PI, scale: 'br'}, // bottom right
      {x: x + width / 2, y, rad: 5, beginA: Math.PI, endA: Math.PI * 2, scale: 't'}, // top middle
      {x, y: y + height / 2, rad: 5, beginA: Math.PI * 0.5, endA: Math.PI * 1.5, scale: 'l'}, // left middle
      {x: x + width / 2, y: y + height, rad: 5, beginA: 0, endA: Math.PI, scale: 'b'}, // bottom middle
      {x: x + width, y: y + height / 2, rad: 5, beginA: -Math.PI * 0.5, endA: Math.PI * 0.5, scale: 'r'} // right middle
    ]
  }

  // function to move position of selection
  move = function (topLeftPoint, bottomRighPoint) {
    this.topLeftPoint = topLeftPoint
    this.width = bottomRighPoint.x - topLeftPoint.x
    this.height = bottomRighPoint.y - topLeftPoint.y
    this._updateCircleProperties()
  }

  select = function () {
    this._drawRect('gray')
    this._drawCircles('gray')
  }

  _drawRect = function (color, margin = 0, func) {
    const {x, y} = this.topLeftPoint
    this.canvasContext.save()
    this.canvasContext.beginPath()
    this.canvasContext.lineWidth = 2
    this.canvasContext.strokeStyle = color
    this.canvasContext.setLineDash([5, 3]) // dashes are 5px and spaces are 3px
    // draw rect
    this.canvasContext.rect(x - margin, y - margin, this.width + margin * 2, this.height + margin * 2)
    func != null && func()
    this.canvasContext.stroke()
    this.canvasContext.restore()
  }

  containsPoint = function (point) {
    let toRet = false
    this._drawRect('rgba(0,0,0,0)', 10, () => {
      if (this.canvasContext.isPointInPath(point.x, point.y)) {
        toRet = true
      }
    })
    return toRet
  }

  _drawCircles = function (color, margin = 0, beginDeg, endDeg, func) {
    this.canvasContext.save()
    this.canvasContext.strokeStyle = color
    this.canvasContext.lineWidth = 2
    this._circleProperties.forEach(prop => {
      this.canvasContext.beginPath()
      this.canvasContext.arc(prop.x, prop.y, prop.rad + margin, beginDeg ? beginDeg != null : prop.beginA, endDeg != null ? endDeg : prop.endA)
      func && func(prop)
      this.canvasContext.stroke()
    })

    this.canvasContext.restore()
  }

  // function to check if user has the pointer over some circle to scale figure
  isScaling = function (point) {
    let toRet = false
    this._drawCircles('rgba(0,0,0,0)', 4, 0, Math.PI * 2, (prop) => {
      if (this.canvasContext.isPointInPath(point.x, point.y)) {
        toRet = prop.scale
      }
    })
    return toRet
  }
}
