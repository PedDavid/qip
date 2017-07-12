import React from 'react'

import {
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
import {Persist, PersistType} from './../../util/Persist'
// import {Figure, FigureStyle} from './../../model/Figure'

// check if it's authenticated

// if not, get data from localstorage
/* if (window.localStorage.getItem('figures') === null && window.localStorage.getItem('pen') === null && window.localStorage.getItem('eraser') === null) {
  const tempGrid = new Grid([], -1)
  window.localStorage.setItem('figures', JSON.stringify(tempGrid.getFigures()))
  window.localStorage.setItem('currFigureId', JSON.stringify(tempGrid.getCurrentFigureId()))
  window.localStorage.setItem('pen', JSON.stringify(new Pen(tempGrid, 'black', 5)))
  window.localStorage.setItem('eraser', JSON.stringify(new Eraser(tempGrid, 20)))
}

const figures = JSON.parse(window.localStorage.getItem('figures'))
const currFigureId = JSON.parse(window.localStorage.getItem('currFigureId'))
const grid = new Grid(figures, currFigureId)
const tempPen = JSON.parse(window.localStorage.getItem('pen'))
const pen = new Pen(grid, tempPen.color, tempPen.width)
const tempEraser = JSON.parse(window.localStorage.getItem('eraser'))
const eraser = new Eraser(grid, tempEraser.width)
*/

const grid = new Grid([], -1)
const pen = new Pen(grid, 'black', 5)
const eraser = new Eraser(grid, 5)
export default class Board extends React.Component {
  state = {
    showCleanModal: false,
    showUserModal: false,
    currTool: pen,
    favorites: [pen, eraser] // obtain favorites from server
  }
  toolsConfig = new ToolsConfig(defaultToolsConfig)

  listeners = {
    // currently, this.perists is not defined but it will on componentDidMount
    onDown: event => this.state.currTool.onPress(event, 1),
    onUp: event => this.state.currTool.onPressUp(event, this.persist),
    onMove: event => this.state.currTool.onSwipe(event, 1),
    onOut: event => this.state.currTool.onOut(event, this.persist)
  }

  componentDidMount () {
    console.log('current board id: ' + this.props.match.params.board)
    const boardId = this.props.match.params.board
    const persistType = PersistType.WebSockets
    this.persist = new Persist(persistType, boardId, this.canvasContext, grid)

    // draw initial grid
    grid.draw(this.canvasContext, 1)
  }

  componentWillMount () {
    // necessary procedure to avoid bug
    this.toolsConfig.updatePrevTool(eraser)
    this.toolsConfig.updatePrevTool(pen)
  }

  addFavorite (tool) {
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
    window.localStorage.setItem('figures', '[]')
    window.localStorage.setItem('currFigureId', '-1')
    grid.clean(this.canvasContext)
    this.toggleCleanModal()
  }
  toggleCleanModal = () => {
    this.setState(prevState => { return { showCleanModal: !prevState.showCleanModal } })
  }

  refCallback = (ref) => {
    this.canvasContext = ref.canvas.getContext('2d')
  }

  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.xpto}>
        <SideBarOverlay grid={grid} changeCurrentTool={this.changeCurrentTool.bind(this)} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite.bind(this)}
          removeFavorite={this.removeFavorite.bind(this)} toggleUserModal={this.toggleUserModal}>
          <Canvas ref={this.refCallback} width={1200} height={800} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <CleanBoardModal cleanCanvas={this.cleanCanvas} closeModal={this.toggleCleanModal} visible={this.state.showCleanModal} />
        <Route path='/signin' component={EnterUserModal} />
      </div>
    )
  }
}
