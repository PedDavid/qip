import React from 'react'

import {
  BrowserRouter as Router,
  Route
} from 'react-router-dom'

import SideBarOverlay from './components/SideBarOverlay'
import Canvas from './components/Canvas'
import CleanBoardModal from './components/Modals/CleanBoardModal'
import EnterUserModal from './components/Modals/EnterUserModal'
import styles from './styles.scss'

import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Grid from './../../model/Grid'
import ToolsConfig from './../../model/ToolsConfig'
import defaultToolsConfig from './../../public/configFiles/defaultTools'
import {Figure, FigureStyle} from './../../model/Figure'

const grid = new Grid([], 300, 300, 0)
const pen = new Pen(grid, 'black', 5) // obtain current tools from server
const eraser = new Eraser(grid, 20)

const scheme = document.location.protocol === 'https:' ? 'wss' : 'ws'
const port = document.location.port ? (':' + 53379) : ''
const connectionUrl = scheme + '://' + document.location.hostname + port + '/ws' + '?id=0'
const socket = new WebSocket(connectionUrl)

socket.onopen = (event) => {
  console.log('DONE')
}

export default class Board extends React.Component {
  state = {
    showCleanModal: false,
    showUserModal: false,
    currTool: pen,
    favorites: [pen, eraser] // obtain favorites from server
  }
  toolsConfig = new ToolsConfig(defaultToolsConfig)

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

  componentWillMount () {
    this.toolsConfig.updatePrevTool(eraser)
    this.toolsConfig.updatePrevTool(pen)
  }

  addFavorite = (tool) => {
    this.setState(() => this.state.favorites.push(tool)) // not needed to change prevState
  }
  removeFavorite = (tool) => {
    this.setState((prevState) => {
      const index = prevState.favorites.indexOf(tool)
      if (index > -1) {
        prevState.favorites.splice(index, 1)
      }
    })
  }
  changeCurrentTool = (tool) => {
    this.toolsConfig.updatePrevTool(this.state.currTool)
    this.setState({currTool: tool})
  }
  cleanCanvas = () => {
    grid.clean(this.canvasContext)
    this.toggleCleanModal()
  }
  toggleCleanModal = () => {
    window.alert(2)
    this.setState(prevState => { return { showCleanModal: !prevState.showCleanModal } })
  }

  refCallback = (ref) => {
    this.canvasContext = ref.canvas.getContext('2d')
  }

  render () {
    return (
      <Router>
        <div className={styles.xpto}>
          <SideBarOverlay grid={grid} changeCurrentTool={this.changeCurrentTool} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
            currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite}
            removeFavorite={this.removeFavorite} toggleUserModal={this.toggleUserModal}>
            <Canvas ref={this.refCallback} width={1200} height={800} {...this.listeners}>
              HTML5 Canvas not supported
            </Canvas>
          </SideBarOverlay>
          <CleanBoardModal cleanCanvas={this.cleanCanvas} closeModal={this.toggleCleanModal} visible={this.state.showCleanModal} />
          <Route path='/signin' component={EnterUserModal} />
        </div>
      </Router>
    )
  }
}
