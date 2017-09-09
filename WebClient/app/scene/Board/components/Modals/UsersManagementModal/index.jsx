import React from 'react'
import { Modal, Label, Table } from 'semantic-ui-react'

export default class UsersManagementModal extends React.Component {
  render () {
    return (
      <Modal open={this.props.visible} onClose={this.props.closeModal}>
        <Modal.Header>Users Management</Modal.Header>
        <Modal.Content >
          <Modal.Description>
            <Table celled>
              <Table.Header>
                <Table.Row>
                  <Table.HeaderCell>Header</Table.HeaderCell>
                  <Table.HeaderCell colSpan='2'>Permission</Table.HeaderCell>
                </Table.Row>
              </Table.Header>

              <Table.Body>
                <Table.Row>
                  <Table.Cell>
                    <Label ribbon>First</Label>
                  </Table.Cell>
                  <Table.Cell>Cell</Table.Cell>
                  <Table.Cell>Cell</Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell>Cell</Table.Cell>
                  <Table.Cell>Cell</Table.Cell>
                  <Table.Cell>Cell</Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell>Cell</Table.Cell>
                  <Table.Cell>Cell</Table.Cell>
                  <Table.Cell>Cell</Table.Cell>
                </Table.Row>
              </Table.Body>
            </Table>
          </Modal.Description>
        </Modal.Content>
      </Modal>
    )
  }
}
