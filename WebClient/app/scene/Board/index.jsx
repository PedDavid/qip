import React from 'react'

import {
  Route
} from 'react-router-dom'

import SideBarOverlay from './components/SideBarOverlay'
import Canvas from './components/Canvas'
import CleanBoardModal from './components/Modals/CleanBoardModal'
import EnterUserModal from './components/Modals/EnterUserModal'
import ShareBoardModal from './components/Modals/ShareBoardModal'
import styles from './styles.scss'
import fetch from 'isomorphic-fetch'

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

export default class Board extends React.Component {
  grid = new Grid([], -1)
  defaultPen = new Pen(this.grid, 'black', 5)
  defaultEraser = new Eraser(this.grid, 5)

  state = {
    showCleanModal: false,
    showUserModal: false,
    showShareModal: false,
    currTool: this.defaultPen,
    favorites: [], // obtain favorites from server
    currentBoardId: null
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
    this.persist = new Persist(persistType, boardId, this.canvasContext, this.grid)

    if (boardId != null) {
      // fetch data of current board
      Promise.all([
        fetch(`http://localhost:57251/api/boards/${boardId}/figures/lines`, {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          method: 'GET'
        }),
        fetch(`http://localhost:57251/api/boards/${boardId}/figures/images`, {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          method: 'GET'
        })
      ]).then(responses => {
        if (responses.some(res => res.status >= 400)) {
          throw new Error('Bad response from server')
        }
        return {lines: responses[0].json(), images: responses[1].json()}
      }).then(figures => {
        const ids = figures.map(figure => figure.map(innerFig => innerFig.id))
        var max = ids.reduce((a, b) => {
          return Math.max(a, b)
        })
        // todo: make possible to get images too
        this.grid = new Grid(figures.lines, max)
        // const pen = new Pen(this.grid, 'black', 5)
        // const eraser = new Eraser(this.grid, 5)

        // update current board id and connect to websockets
        this.updateBoardId(boardId)
      }).catch(err => {
        console.log(err)
        console.log('The board id passed as parameter is not valid')
        this.props.history.push('/') // change current location programmatically in case of error
      })
    }

    // draw initial grid
    this.grid.draw(this.canvasContext, 1)
  }

  componentWillMount () {
    // necessary procedure to avoid bug
    this.toolsConfig.updatePrevTool(this.defaultPen)
    this.toolsConfig.updatePrevTool(this.defaultEraser)
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
    this.grid.clean(this.canvasContext)
    this.toggleCleanModal()
  }
  toggleCleanModal = () => {
    this.setState(prevState => { return { showCleanModal: !prevState.showCleanModal } })
  }

  toggleShareModal = () => {
    this.setState(prevState => { return { showShareModal: !prevState.showShareModal } })
  }

  refCallback = (ref) => {
    this.canvasContext = ref.canvas.getContext('2d')
  }

  updateBoardId = (id) => {
    this.setState({
      currentBoardId: id
    })
    this.persist.connect(id)
  }

  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.xpto}>
        <SideBarOverlay grid={this.grid} changeCurrentTool={this.changeCurrentTool.bind(this)} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite.bind(this)}
          removeFavorite={this.removeFavorite.bind(this)} toggleUserModal={this.toggleUserModal} toggleShareModal={this.toggleShareModal}>
          <Canvas ref={this.refCallback} width={1200} height={800} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <CleanBoardModal cleanCanvas={this.cleanCanvas} closeModal={this.toggleCleanModal} visible={this.state.showCleanModal} />
        <ShareBoardModal boardId={this.state.currentBoardId} visible={this.state.showShareModal} closeModal={this.toggleShareModal} updateCurrentBoard={this.updateBoardId} />
        <Route path='/signin' component={EnterUserModal} />
      </div>
    )
  }
}
