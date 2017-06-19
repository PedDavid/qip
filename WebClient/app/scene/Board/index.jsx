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
const pen = new Pen(grid, 'black', 5)
const eraser = new Eraser(grid, 20)

const defaultTools = [
  new Pen(grid, 'black', 4),
  new Pen(grid, 'red', 4),
  new Eraser(grid, 10)
]

export default class Board extends React.Component {
  state = {
    showModal: false,
    currTool: pen,
    favorites: [pen, eraser]
  }

  listeners = {
    onDown: event => this.state.currTool.onPress(event, 1),
    onUp: event => this.state.currTool.onPressUp(event),
    onMove: event => this.state.currTool.onSwipe(event, 1),
    onOut: event => this.state.currTool.onOut(event)
  }
  addFavorite (tool) {
    this.setState(() => this.state.favorites.push(tool)) // not needed to change prevState
  }

  onPaste = (event) => {
    console.log(event)
    this.setState({clipboard: 'something'})
  }
  onKeyDown = (event) => {
    console.log(event.keyCode)
    console.log(event)
  }
  changeCurrentTool (tool) {
    console.log(tool)
    this.setState({currTool: tool})
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
        {/* todo: why cant i send {...listeners} instead of listeners={listeners} ??? */}
        <SideBarOverlay grid={grid} changeCurrentTool={this.changeCurrentTool.bind(this)} favorites={this.state.favorites} defaultTools={defaultTools}
          currTool={this.state.currTool} cleanCanvas={this.openModal.bind(this)} addFavorite={this.addFavorite.bind(this)}>
          <Canvas ref='canvas' grid={grid} width={1200} height={800} listeners={this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <Modalx cleanCanvas={this.cleanCanvas.bind(this)} closeModal={this.closeModal.bind(this)} visible={this.state.showModal} />
      </div>
    )
  }
}
