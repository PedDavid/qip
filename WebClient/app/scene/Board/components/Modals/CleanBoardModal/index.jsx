import React from 'react'
import { Modal, Button } from 'semantic-ui-react'

export default function CleanBoardModal ({open, cleanCanvas, onClose}) {
  return (
    <Modal size='small' onClose={onClose} open={open} closeIcon>
      <Modal.Header>
        Clear All Board
      </Modal.Header>
      <Modal.Content>
        <p>You are about to clear all board. Are you sure?</p>
      </Modal.Content>
      <Modal.Actions>
        <Button onClick={cleanCanvas} positive content='Yes' />
        <Button onClick={onClose} negative content='No' />
      </Modal.Actions>
    </Modal>
  )
}
