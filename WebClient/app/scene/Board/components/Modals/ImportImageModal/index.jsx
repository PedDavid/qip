import React from 'react'
import { Modal, Button } from 'semantic-ui-react'

export default function ImportImageModal (props) {
  return (
    <Modal size='small' open={props.open} onClose={props.onClose} closeIcon>
      <Modal.Header>
        Import Image
      </Modal.Header>
      <Modal.Content>
        <p>You are about to clear all board. Are you sure?</p>
        <label htmlFor='file-upload'>
          <Button> Upload File </Button>
        </label>
        <input accept='image/*' id='file-upload' onChange={() => console.log('onchange')} style={{zIndex: 1000, visibility: 'hidden'}} type='file' />
      </Modal.Content>
      <Modal.Actions />
    </Modal>
  )
}
