// @flow

import React from 'react'

type PointerEvents = {
  onDown?: any => void,
  onUp?: any => void,
  onMove?: any => void,
  onOut?: any => void
}

export default class Canvas extends React.Component {
  props: PointerEvents
  canvas: window.EventListener

  componentDidMount () {
    if (this.props.onDow) {
      this.canvas.addEventListener('pointerdown', this.props.onDown, false)
    }
    if (this.props.onUp) {
      this.canvas.addEventListener('pointerup', this.props.onUp, false)
    }
    if (this.props.onMove) {
      this.canvas.addEventListener('pointermove', this.props.onMove, false)
    }
    if (this.props.onOut) {
      this.canvas.addEventListener('pointerout', this.props.onOut, false)
    }
  }

  render () {
    const { onDown, onUp, onMove, onOut, ...props } = this.props
    const style = { border: '1px solid #000000' } // TODO(PedDavid): Delete this, used only for debug purposes
    return <canvas ref={canvas => { this.canvas = canvas }} {...props} style={style} />
  }
}
