// @flow

import React from 'react'
import SideBarOverlay from './components/SideBarOverlay'
import Canvas from './components/Canvas'
import Modalx from './components/Modal'
import styles from './styles.scss'

import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Grid from './../../model/Grid'

const grid = new Grid([], 300, 300, 0)
const pen = new Pen(grid, 'black', 4)
const eraser = new Eraser(grid, 20)
let currTool = pen

const favorites = [1, 2, 3, 4]

const listeners = {
  onDown: event => currTool.onPress(event, 1),
  onUp: event => currTool.onPressUp(event),
  onMove: event => currTool.onSwipe(event, 1),
  onOut: event => currTool.onOut(event)
}

const defaultTools = [
  new Pen(grid, 'black', 4),
  new Pen(grid, 'red', 4),
  new Eraser(grid, 10)
]

export default class Board extends React.Component {
  state = {showModal: false}
  onPaste = (event) => {
    console.log(event)
    this.setState({clipboard: 'something'})
  }
  onKeyDown = (event) => {
    console.log(event.keyCode)
    console.log(event)
  }
  cleanCanvas () {
    this.refs.canvas.cleanCanvas()
    this.closeModal()
  }
  openModal () {
    this.setState({showModal: true})    
  }
  closeModal () {
    this.setState({showModal: false})    
  }
  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.xpto}>
        {/*why cant i send {...listeners} instead of listeners={listeners} ??? */}
        <SideBarOverlay favorites={favorites} defaultTools={defaultTools} currTool={currTool} cleanCanvas={this.openModal.bind(this)}>
          <Canvas ref="canvas" grid={grid} width={1200} height={800} listeners={listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <Modalx cleanCanvas={this.cleanCanvas.bind(this)} closeModal={this.closeModal.bind(this)} visible={this.state.showModal}/>
      </div>
    )
  }
}
