// @flow

import React from 'react'
import { Modal, Button } from 'semantic-ui-react'

export default class Modalx extends React.Component {
  render () {
    return (
      <Modal size='small' open={this.props.visible}>
        <Modal.Header>
          Clear All Board
        </Modal.Header>
        <Modal.Content>
          <p>You are about to clear all board. Are you sure?</p>
        </Modal.Content>
        <Modal.Actions>
          <Button onClick={this.props.cleanCanvas} positive content='Yes' />
          <Button onClick={this.props.closeModal} negative content='No' />
        </Modal.Actions>
      </Modal>
    )
  }
}
