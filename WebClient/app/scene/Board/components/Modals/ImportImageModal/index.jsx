import React from 'react'
import { Modal, Input, Divider } from 'semantic-ui-react'

export default function ImportImageModal (props) {
  return (
    <Modal size='small' open={props.open} onClose={props.onClose} closeIcon>
      <Modal.Header>
        Import Image
      </Modal.Header>
      <Modal.Content>
        <Input placeholder='Paste URL' fluid />
        <Divider horizontal>Or</Divider>
        <input accept='image/*' id='file-upload' onChange={() => console.log('onchange')} type='file' />
      </Modal.Content>
    </Modal>
  )
}
