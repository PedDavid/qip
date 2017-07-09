import React from 'react'
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

const grid = new Grid([], 300, 300, 0)
const pen = new Pen(grid, 'black', 5) // obtain current tools from server
const eraser = new Eraser(grid, 20)

export default class Board extends React.Component {
  state = {
    showCleanModal: false,
    showUserModal: false,
    currTool: pen,
    favorites: [pen, eraser] // obtain favorites from server
  }
  toolsConfig = new ToolsConfig(defaultToolsConfig)

  listeners = {
    onDown: event => this.state.currTool.onPress(event, 1),
    onUp: event => this.state.currTool.onPressUp(event),
    onMove: event => this.state.currTool.onSwipe(event, 1),
    onOut: event => this.state.currTool.onOut(event)
  }

  componentWillMount () {
    this.toolsConfig.updatePrevTool(eraser)
    this.toolsConfig.updatePrevTool(pen)
  }

  addFavorite (tool) {
    this.setState(() => this.state.favorites.push(tool)) // not needed to change prevState
  }
  removeFavorite (tool) {
    this.setState((prevState) => {
      const index = prevState.favorites.indexOf(tool)
      if (index > -1) {
        prevState.favorites.splice(index, 1)
      }
    })
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
    this.toolsConfig.updatePrevTool(this.state.currTool)
    this.setState({currTool: tool})
  }
  cleanCanvas () {
    grid.clean(this.canvasContext)
    this.toggleModal()
  }
  toggleModal () {
    this.setState(prevState => { return { showCleanModal: !prevState.showCleanModal } })
  }

  toggleUserModal = () => {
    this.setState(prevState => { return { showUserModal: !prevState.showUserModal } })
  }

  refCallback = (ref) => {
    this.canvasContext = ref.canvas.getContext('2d')
  }

  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.xpto}>
        <SideBarOverlay grid={grid} changeCurrentTool={this.changeCurrentTool.bind(this)} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleModal.bind(this)} addFavorite={this.addFavorite.bind(this)}
          removeFavorite={this.removeFavorite.bind(this)} toggleUserModal={this.toggleUserModal}>
          <Canvas ref={this.refCallback} width={1200} height={800} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <CleanBoardModal cleanCanvas={this.cleanCanvas.bind(this)} closeModal={this.toggleModal.bind(this)} visible={this.state.showCleanModal} />
        <EnterUserModal visible={this.state.showUserModal} toggleUserModal={this.toggleUserModal} />
      </div>
    )
  }
}
