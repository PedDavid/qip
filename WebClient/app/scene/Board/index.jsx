// @flow

import React from 'react'
import SideBarOverlay from './components/SideBarOverlay'
import Canvas from './components/Canvas'

import { Modal, Button, Icon, Header } from 'semantic-ui-react'

import FavoriteTools from './components/FavoriteTools'

const favorites = [1, 2, 3, 4]

export default class Board extends React.Component {
  state = {clipboard: 'nothing'}
  onPaste = (event) => {
    console.log(event)
    this.setState({clipboard: 'something'})
  }
  onKeyDown = (event) => {
    console.log(event.keyCode)
    console.log(event)
  }
  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown}>
        <Header as='h3'>{this.state.clipboard}</Header>
        <SideBarOverlay>
          <Canvas width={300} height={300}>
            HTML5 Canvas not supported
          </Canvas>
          <FavoriteTools favorites={favorites} />
        </SideBarOverlay>
        <Modal trigger={<Button>Basic Modal</Button>} basic size='small'>
          <Header icon='trash' content='Clear Board' />
          <Modal.Content>
            <p>Are you sure?</p>
          </Modal.Content>
          <Modal.Actions>
            <Button basic color='red' inverted>
              <Icon name='remove' /> No
            </Button>
            <Button color='green' inverted>
              <Icon name='checkmark' /> Yes
            </Button>
          </Modal.Actions>
        </Modal>
      </div>
    )
  }
}
