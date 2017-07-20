// import {SimplePoint} from './SimplePoint'

export function Image (imgSrcPoint, imgSrc, imgWidth, imgHeight, id = null) {
  this.id = id
  let srcPoint = imgSrcPoint
  let width = imgWidth
  let height = imgHeight
  let src = imgSrc
  let img

  function createImage () {
    const auxImg = document.createElement('img')
    // const auxImg = new Image()
    auxImg.src = src
    // scale image
    const scale = auxImg.width / 200 // 200 is de default width
    width === undefined && (width = auxImg.width === 0 ? 200 : auxImg.width / scale)
    height === undefined && (height = auxImg.height === 0 ? 200 : auxImg.height / scale)
  }

  createImage()

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
}
