import React from 'react'
import { Modal, Popup, Checkbox, Icon } from 'semantic-ui-react'
import styles from './styles.scss'
import SettingsConfig from './../../../../../util/SettingsConfig.js'

export default class SettingsModal extends React.Component {
  state = {
    fingerMoveSettingDisabled: false,
    dynamicPageSettingState: false,
    fingerMoveSettingState: false
  }
  onClose = () => {
    this.props.closeModal()
  }

  onDynamicPageSettingChange = (event, data) => {
    console.log(data.checked)
    if (data.checked === false) {
      this.props.updateSettings(false, SettingsConfig.fingerMoveSettingIdx)
    }
    this.setState({
      fingerMoveSettingDisabled: !data.checked
    })
    this.props.updateSettings(data.checked, SettingsConfig.dynamicPageSettingIdx)
  }

  onFingerMoveSettingChange = (event, data) => {
    console.log(data.checked)
    this.setState({
      fingerMoveSettingDisabled: !data.checked
    })
    this.props.updateSettings(data.checked, SettingsConfig.fingerMoveSettingIdx)
  }

  componentWillUpdate = () => {
    this.fingerMoveSettingState = this.props.settings[SettingsConfig.fingerMoveSettingIdx]
    this.dynamicPageSettingState = this.props.settings[SettingsConfig.dynamicPageSettingIdx]
  }

  componentWillMount = () => {
    this.fingerMoveSettingState = this.props.settings[SettingsConfig.fingerMoveSettingIdx]
    this.dynamicPageSettingState = this.props.settings[SettingsConfig.dynamicPageSettingIdx]
  }

  render () {
    return (
      <Modal size='small' open={this.props.visible} onClose={this.onClose}>
        <Modal.Header>
          <Icon name='setting' />
          Settings
        </Modal.Header>
        <Modal.Content>
          <div className={styles.settingStyle}>
            <span>
              Dynamic Page
              <Popup
                trigger={
                  <Icon className={styles.cbStyle} name='question circle outline' />
                }
                content='Dynamic Page allow you to use the canvas as if its size were infinite'
                basic
              />
              <Checkbox checked={this.dynamicPageSettingState} onChange={this.onDynamicPageSettingChange} className={styles.cbStyle} toggle />
            </span>
          </div>
          <div className={styles.settingStyle}>
            <span>
              Finger Move
              <Popup
                trigger={
                  <Icon className={styles.cbStyle} name='question circle outline' />
                }
                content='If using Dynamic Page, allow you to use figer to move page'
                basic
              />
              <Checkbox checked={this.fingerMoveSettingState} disabled={this.state.fingerMoveSettingDisabled} onChange={this.onFingerMoveSettingChange} className={styles.cbStyle} toggle />
            </span>
          </div>
        </Modal.Content>
      </Modal>
    )
  }
}
