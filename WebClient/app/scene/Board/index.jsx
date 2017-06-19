import React from 'react'
import SideBarOverlay from './components/SideBarOverlay'
import Canvas from './components/Canvas'
import Modalx from './components/Modal'
import styles from './styles.scss'

import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Grid from './../../model/Grid'
import {Figure, FigureStyle} from './../../model/Figure'

const grid = new Grid([], 300, 300, 0)
const pen = new Pen(grid, 'black', 5)
const eraser = new Eraser(grid, 20)

const defaultTools = [
  new Pen(grid, 'black', 4),
  new Pen(grid, 'red', 4),
  new Eraser(grid, 10)
]

const scheme = document.location.protocol === 'https:' ? 'wss' : 'ws'
const port = document.location.port ? (':' + 53379) : ''
const connectionUrl = scheme + '://' + document.location.hostname + port + '/ws' + '?id=0'
const socket = new WebSocket(connectionUrl)

socket.onopen = (event) => {
  console.log('DONE')
}

export default class Board extends React.Component {
  state = {
    showModal: false,
    currTool: pen,
    favorites: [pen, eraser]
  }

  componentDidMount () {
    socket.onmessage = (event) => {
      console.log(event.data)
      const {type, payload} = JSON.parse(event.data)
      console.log(payload)
      switch (type) {
        case 'INSERT_FIGURE':
          const figStyle = new FigureStyle(payload.figureStyle.color, 1)
          const newFigure = new Figure(figStyle, payload.id)
          newFigure.points = payload.points.map(point => { return { x: point.x, y: point.y, style: {press: 3} } })
          grid.addFigure(newFigure)
          grid.draw(this.refs.canvas.canvas.getContext('2d'), 1)
      }
    }
  }

  listeners = {
    onDown: event => this.state.currTool.onPress(event, 1),
    onUp: event => this.state.currTool.onPressUp(event, socket),
    onMove: event => this.state.currTool.onSwipe(event, 1),
    onOut: event => this.state.currTool.onOut(event, socket)
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
    grid.clean(this.canvasContext)
    this.toggleModal()
  }
  toggleModal () {
    this.setState(prevState => { return { showModal: !prevState.showModal } })
  }

  refCallback = (ref) => {
    this.canvasContext = ref.canvas.getContext('2d')
  }

  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.xpto}>
        <SideBarOverlay grid={grid} changeCurrentTool={this.changeCurrentTool.bind(this)} favorites={this.state.favorites} defaultTools={defaultTools}
          currTool={this.state.currTool} cleanCanvas={this.toggleModal.bind(this)} addFavorite={this.addFavorite.bind(this)}>
          <Canvas ref={this.refCallback} width={1200} height={800} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <Modalx cleanCanvas={this.cleanCanvas.bind(this)} closeModal={this.toggleModal.bind(this)} visible={this.state.showModal} />
      </div>
    )
  }
}
