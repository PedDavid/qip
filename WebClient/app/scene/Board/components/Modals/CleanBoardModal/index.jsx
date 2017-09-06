import React from 'react'
import { Modal, Button } from 'semantic-ui-react'

export default function CleanBoardModal ({open, cleanCanvas, closeModal}) {
  return (
    <Modal size='small' open={open}>
      <Modal.Header>
        Clear All Board
      </Modal.Header>
      <Modal.Content>
        <p>You are about to clear all board. Are you sure?</p>
      </Modal.Content>
      <Modal.Actions>
        <Button onClick={cleanCanvas} positive content='Yes' />
        <Button onClick={closeModal} negative content='No' />
      </Modal.Actions>
    </Modal>
  )
}
