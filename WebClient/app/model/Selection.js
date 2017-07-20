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

  move = function (topLeftPoint, bottomRighPoint) {
    this.topLeftPoint = topLeftPoint
    this.width = bottomRighPoint.x - topLeftPoint.x
    this.height = bottomRighPoint.y - topLeftPoint.y
    this._updateCircleProperties()
  }

  select = function () {
    this._drawRect()
    this._drawCircles()
  }

  _drawRect = function () {
    const {x, y} = this.topLeftPoint
    this.canvasContext.save()
    this.canvasContext.beginPath()
    this.canvasContext.strokeStyle = 'gray'
    this.canvasContext.setLineDash([5, 3]) // dashes are 5px and spaces are 3px
    // draw rect
    this.canvasContext.rect(x, y, this.width, this.height)
    this.canvasContext.stroke()
    this.canvasContext.restore()
  }

  _drawCircles = function () {
    this.canvasContext.save()
    this.canvasContext.strokeStyle = 'gray'
    this._circleProperties.forEach(prop => {
      this.canvasContext.beginPath()
      this.canvasContext.arc(prop.x, prop.y, prop.rad, prop.beginA, prop.endA)
      this.canvasContext.stroke()
    })

    this.canvasContext.restore()
  }

  // function to check if user has the pointer over some circle to scale figure
  isScaling = function (point) {
    const margin = 4
    let toRet = false
    this.canvasContext.save()
    this.canvasContext.strokeStyle = 'rgba(0,0,0,0)'
    for (let key in this._circleProperties) {
      const prop = this._circleProperties[key]
      this.canvasContext.beginPath()
      this.canvasContext.arc(prop.x, prop.y, prop.rad + margin, 0, Math.PI * 2)
      this.canvasContext.stroke()
      if (this.canvasContext.isPointInPath(point.x, point.y)) {
        toRet = prop.scale
        break
      }
    }
    this.canvasContext.restore()
    return toRet
  }
}
