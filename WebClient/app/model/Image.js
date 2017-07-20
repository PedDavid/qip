import {SimplePoint} from './SimplePoint'
import {Rect} from './Rect'

export function Image (imgSrcPoint, imgSrc, imgWidth, imgHeight, id = null) {
  this.id = id
  let srcPoint = imgSrcPoint
  let width = imgWidth
  let height = imgHeight
  let src = imgSrc
  let img

  function scaleImage () {
    const auxImg = document.createElement('img')
    // const auxImg = new Image()
    auxImg.src = src
    // scale image
    const scale = auxImg.width / 200 // 200 is de default width
    width === undefined && (width = auxImg.width === 0 ? 200 : auxImg.width / scale)
    height === undefined && (height = auxImg.height === 0 ? 200 : auxImg.height / scale)
    auxImg.onload = (e) => {
      img = auxImg
    }
  }

  scaleImage()

  this.getTopLeftPoint = function () {
    return srcPoint
  }

  this.getBottomRightPoint = function () {
    return new SimplePoint(srcPoint.x + width, srcPoint.y + height)
  }

  this.getSrcPoint = function () {
    return srcPoint
  }

  this.setSrcPoint = function (point) {
    srcPoint = point
  }

  this.getWidth = function () {
    return width
  }

  this.getHeight = function () {
    return height
  }

  this.getSrc = function () {
    return src
  }

  this.draw = function (ctx, currScale) {
    if (img === undefined) {
      // todo: (simao) isto não deveria ser através de DOM mas não consegui fazer de outra forma (p.e. com new Image()).
      const leImg = document.createElement('img')
      leImg.onload = (e) => {
        img = leImg
        ctx.drawImage(img, srcPoint.x, srcPoint.y, width, height)
      }
      leImg.src = src
    } else {
      ctx.drawImage(img, srcPoint.x, srcPoint.y, width, height)
    }
  }

  this.containsPoint = function (point, event) {
    const canvasContext = event.target.getContext('2d')
    const margin = 10
    const imgHeight = this.getHeight()
    const initPoint = new SimplePoint(this.getSrcPoint().x - margin, this.getSrcPoint().y + imgHeight / 2)
    const endPoint = new SimplePoint(this.getSrcPoint().x + this.getWidth() + margin, this.getSrcPoint().y + imgHeight / 2)

    const rect = new Rect(initPoint, endPoint, imgHeight + margin * 2, canvasContext)

    return rect.contains(new SimplePoint(point.x, point.y))
  }

  this.scale = function (scaleType, offsetPoint) {
    switch (scaleType) {
      case 'l':
        this._scaleLeft(offsetPoint)
        break
      case 'r':
        this._scaleRight(offsetPoint)
        break
      case 't':
        this._scaleTop(offsetPoint)
        break
      case 'b':
        this._scaleBottom(offsetPoint)
        break
      case 'tl':
        this._scaleLeft(offsetPoint)
        this._scaleTop(offsetPoint)
        break
      case 'tr':
        this._scaleTop(offsetPoint)
        this._scaleRight(offsetPoint)
        break
      case 'bl':
        this._scaleLeft(offsetPoint)
        this._scaleBottom(offsetPoint)
        break
      case 'br':
        this._scaleBottom(offsetPoint)
        this._scaleRight(offsetPoint)
        break
    }
  }

  this._scaleTop = function (offsetPoint) {
    srcPoint.y = srcPoint.y + offsetPoint.y
    height = height - offsetPoint.y
  }
  this._scaleBottom = function (offsetPoint) {
    height = height + offsetPoint.y
  }
  this._scaleLeft = function (offsetPoint) {
    srcPoint.x = srcPoint.x + offsetPoint.x
    width = width - offsetPoint.x
  }
  this._scaleRight = function (offsetPoint) {
    width = width + offsetPoint.x
  }
}
