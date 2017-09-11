import React from 'react'
import { Modal, Input, Divider, Button, Message } from 'semantic-ui-react'

const MAX_FILE_SIZE_MB = 10
const MAX_FILE_SIZE = MAX_FILE_SIZE_MB * 1024 * 1024

export default class ImportImageModal extends React.Component {
  state = {
    url: '',
    fileSize: 0
  }

  onClick = (event) => {
    this.props.onClose()
    this.props.onImageLoad(this.state.url)
  }

  onUpload = (event) => {
    const file = event.target.files[0]
    this.setState({fileSize: file.size})
    if (!file) {
      return
    } else if (file.size > MAX_FILE_SIZE) {
      console.error(`File size exceeded: ${file.size} (max ${MAX_FILE_SIZE})`)
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
          <p>Max file size: {MAX_FILE_SIZE_MB} MB</p>
        </Modal.Content>
        <Message error hidden={this.state.fileSize < MAX_FILE_SIZE}>
          File size exceeded ({Math.round(this.state.fileSize / (1024 * 1024))} MB)
        </Message>
      </Modal>
    )
  }
}
