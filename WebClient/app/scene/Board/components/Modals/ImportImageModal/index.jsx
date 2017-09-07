import React from 'react'
import { Modal, Input, Divider, Button } from 'semantic-ui-react'

export default class ImportImageModal extends React.Component {
  state = {
    url: ''
  }

  onClick = (event) => {
    this.props.onClose()
    this.props.onImageLoad(this.state.url)
  }

  onUpload = (event) => {
    const file = event.target.files[0]
    if (!file) {
      return
    }
    const reader = new FileReader()
    reader.onload = (event) => {
      this.props.onClose()
      this.props.onImageLoad(reader.result)
    }
    reader.readAsDataURL(file)
  }

  onInput = (event) => {
    this.setState({url: event.target.value})
  }

  render () {
    return (
      <Modal size='small' open={this.props.open} onClose={this.props.onClose} closeIcon>
        <Modal.Header>
          Import Image
        </Modal.Header>
        <Modal.Content>
          <Input placeholder='Paste URL' fluid onInput={this.onInput} action={<Button onClick={this.onClick}>Import</Button>} />
          <Divider horizontal>Or</Divider>
          <input accept='image/*' id='file-upload' onChange={this.onUpload} type='file' />
        </Modal.Content>
      </Modal>
    )
  }
}
