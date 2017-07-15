// @flow

import React from 'react'
import { Modal, Button, Icon, Input } from 'semantic-ui-react'
import fetch from 'isomorphic-fetch'

const baseUrl = 'http://localhost:8080/'

export default class ShareBoardModal extends React.Component {
  state = {
    boardUrl: this.props.boardId != null ? baseUrl + this.props.boardId : '',
    loading: false,
    error: false
  }
  onClose = () => {
    this.props.closeModal()
  }

  getBoard = () => {
    if (this.state.boardUrl !== '' && !this.state.error) {
      return
    }

    fetch('http://localhost:57059/api/boards', {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      method: 'POST',
      body: JSON.stringify({
        'name': 'Board Name',
        'maxDistPoints': 0
      })
    }).then(response => {
      if (response.status >= 400) {
        throw new Error('Bad response from server')
      }
      return response.json()
    }).then(board => {
      // todo: send current board data to api
      this.props.updateCurrentBoard(board.id)
      this.setState({
        boardUrl: baseUrl + board.id,
        loading: false,
        error: false
      })
    }).catch(err => {
      console.log(err)
      this.setState({
        boardUrl: 'Error: Please contact support',
        loading: false,
        error: true
      })
    })

    this.setState({
      loading: true
    })
  }

  componentWillReceiveProps (nexProps) {
    const boardId = this.props.boardId
    if (boardId !== nexProps.boardUrl) {
      this.setState({
        boardUrl: boardId != null ? baseUrl + this.props.boardId : ''
      })
    }
  }

  render () {
    let btnDisabled = false
    if (this.state.boardUrl !== '' && !this.state.error) {
      btnDisabled = true
    }

    return (
      <Modal size='small' open={this.props.visible} onClose={this.onClose}>
        <Modal.Header>
          <Icon name='share' size='small' />
          Share Your Board
        </Modal.Header>
        <Modal.Content>
          <p>Share Your Board By A URL</p>
          <Button disabled={btnDisabled} style={{float: 'left'}} loading={this.state.loading}
            onClick={this.getBoard} size='medium'>
            Get Url
          </Button>
          <Input readOnly fluid error={this.state.error}
            action={{ color: 'teal', icon: 'copy' }}
            placeholder={this.state.boardUrl} />
        </Modal.Content>
      </Modal>
    )
  }
}
