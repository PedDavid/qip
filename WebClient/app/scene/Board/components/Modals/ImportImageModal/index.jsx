import React from 'react'
import { Modal } from 'semantic-ui-react'

export default function ImportImageModal (props) {
  return (
    <Modal size='small' open={props.open}>
      <Modal.Header>
        Import Image
      </Modal.Header>
      <Modal.Content>
        <p>You are about to clear all board. Are you sure?</p>
      </Modal.Content>
      <Modal.Actions />
    </Modal>
  )
}
