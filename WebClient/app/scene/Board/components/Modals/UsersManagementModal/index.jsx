import React from 'react'
import { Modal, Label, Table, Image, Header, Button, Icon } from 'semantic-ui-react'

export default class UsersManagementModal extends React.Component {
  state = {
    boardUsers: []
  }

  updated = false

  componentDidUpdate () {
    this.fetchInfo()
  }

  fetchInfo () {
    if (!this.props.visible || this.props.persist.boardId == null || this.updated) {
      return
    }
    this.setState({loading: true})
    this.updated = true
    Promise.all([
      this.props.persist.getBoardUsers(this.props.persist.boardId, this.props.auth.getAccessToken()),
      this.props.persist.getBoardInfo(this.props.persist.boardId, this.props.auth.tryGetProfile(), this.props.auth.getAccessToken())
    ]).then(allRes => {
      this.setState({
        boardUsers: allRes[0].filter(user => user.permission !== 3),
        boardInfo: allRes[1],
        loading: false
      })
    }).catch(err => {
      console.log(err)
      this.updated = false
      this.setState({loading: false})
    })
  }

  onClose = () => {
    this.updated = false
    this.props.closeModal()
  }

  updateUserPermission = (props) => {
    const [userIdx, userId, userPermission] = props

    this.setState(prevState => {
      prevState.boardUsers[userIdx].loading = true
    })

    this.props.persist.updateUserPermission(userId, this.props.persist.boardId, userPermission, this.props.auth.getAccessToken())
      .then(res => {
        this.setState(prevState => {
          prevState.boardUsers[userIdx].permission = userPermission
          prevState.boardUsers[userIdx].loading = false
        })
      }).catch(err => {
        this.setState(prevState => {
          prevState.boardUsers[userIdx].loading = false
        })
        console.error(err)
      })
  }

  render () {
    const boardName = this.state.boardInfo != null ? this.state.boardInfo.name : 'Your Board'
    const loadingView = this.state.loading
      ? (<Table.Row>
        <Table.Cell colSpan='3' textAlign='center'>
          <Button basic loading circular />
        </Table.Cell>
      </Table.Row>)
      : null

    return (
      <div>
        <Modal open={this.props.visible} onClose={this.onClose}>
          <Modal.Header>Users Management</Modal.Header>
          <Modal.Content >
            <Modal.Description>
              <Table celled>
                <Table.Header>
                  <Table.Row>
                    <Table.HeaderCell colSpan='3' textAlign='left'>
                      <Label ribbon>{boardName}</Label>
                    </Table.HeaderCell>
                  </Table.Row>
                  <Table.Row>
                    <Table.HeaderCell>Users</Table.HeaderCell>
                    <Table.HeaderCell colSpan='2' textAlign='center'>Permission</Table.HeaderCell>
                  </Table.Row>
                </Table.Header>
                <Table.Body>
                  {this.state.boardUsers.map((boardUser, idx) => {
                    return (
                      <Table.Row key={boardUser.user.id}>
                        <Table.Cell>
                          <Header as='h4' image>
                            <Image avatar src={boardUser.user.picture} />
                            <Header.Content>
                              {boardUser.user.nickname}
                              <Header.Subheader> {boardUser.user.name} </Header.Subheader>
                            </Header.Content>
                          </Header>
                        </Table.Cell>
                        <Table.Cell collapsing textAlign='center'>
                          <Button toggle loading={boardUser.loading} active={boardUser.permission === 2 && !boardUser.loading} onClick={this.updateUserPermission.bind(this, [idx, boardUser.user.id, 2])} icon><Icon name='edit' /></Button>
                        </Table.Cell>
                        <Table.Cell collapsing textAlign='center'>
                          <Button toggle loading={boardUser.loading} active={boardUser.permission === 1 && !boardUser.loading} onClick={this.updateUserPermission.bind(this, [idx, boardUser.user.id, 1])} icon><Icon name='eye' /></Button>
                        </Table.Cell>
                      </Table.Row>
                    )
                  })}
                  {loadingView}
                </Table.Body>
              </Table>
            </Modal.Description>
          </Modal.Content>
        </Modal>
      </div>
    )
  }
}
