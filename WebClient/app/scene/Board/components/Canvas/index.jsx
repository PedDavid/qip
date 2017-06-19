// @flow

import React from 'react'
import styles from './styles.scss'

type PointerEvents = {
  onDown?: any => void,
  onUp?: any => void,
  onMove?: any => void,
  onOut?: any => void
}

export default class Canvas extends React.Component {
  listeners: PointerEvents
  canvas: window.EventListener

  cleanCanvas () {
    console.log(this.props)
    this.props.grid.clean(this.canvas.getContext('2d'))
  }

  componentDidMount () {
    this.componentDidUpdate()
  }

  componentDidUpdate () {
    this.listeners = this.props.listeners
    if (this.listeners == null) { return }
    if (this.listeners.onDown !== null) {
      this.canvas.addEventListener('pointerdown', this.listeners.onDown, false)
    }
    if (this.listeners.onUp !== null) {
      this.canvas.addEventListener('pointerup', this.listeners.onUp, false)
    }
    if (this.listeners.onMove !== null) {
      this.canvas.addEventListener('pointermove', this.listeners.onMove, false)
    }
    if (this.listeners.onOut !== null) {
      this.canvas.addEventListener('pointerout', this.listeners.onOut, false)
    }
  }

  shouldComponentUpdate (nexProps) {
    return nexProps.listeners === this.listeners
  }

  render () {
    return <canvas ref={(canvas) => { this.canvas = canvas }} className={styles.debug} width={this.props.width} height={this.props.height} />
  }
}
