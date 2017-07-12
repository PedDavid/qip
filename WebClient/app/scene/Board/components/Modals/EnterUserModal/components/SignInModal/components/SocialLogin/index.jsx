// @flow
import React from 'react'
import { Icon, Button } from 'semantic-ui-react'
import styles from './styles.scss'
import GoogleLogin from './components/GoogleLogin'

export default class SocialLogin extends React.Component {
  render () {
    const extraStyle = { // not able to be in styles.scss file
      margin: '5px',
      paddingLeft: '14px',
      transition: '0.5s',
      boxShadow: '2px 5px 5px #888888'
    }

    return (
      <div>
        <Button circular color='facebook' style={extraStyle} className={styles.socialBt}>
          <Icon name='facebook' className={styles.socialIcon} />
          <span className={styles.socialName}> Facebook </span>
        </Button>
        <Button circular color='twitter' style={extraStyle} className={styles.socialBt}>
          <Icon name='twitter' className={styles.socialIcon} />
          <span className={styles.socialName}> Twitter </span>
        </Button>
        <GoogleLogin style={extraStyle} className={styles.socialBt} iconClassName={styles.socialIcon} nameClassName={styles.socialName} />
        <Button circular color='purple' style={extraStyle} className={styles.socialBt}>
          <Icon name='slack' className={styles.socialIcon} />
          <span className={styles.socialName}> Slack </span>
        </Button>
        <Button circular color='linkedin' style={extraStyle} className={styles.socialBt}>
          <Icon name='linkedin' className={styles.socialIcon} />
          <span className={styles.socialName}> Linkedin </span>
        </Button>
        <Button circular color='instagram' style={extraStyle} className={styles.socialBt}>
          <Icon name='instagram' className={styles.socialIcon} />
          <span className={styles.socialName}> Instagram </span>
        </Button>
        <Button circular color='youtube' style={extraStyle} className={styles.socialBt}>
          <Icon name='youtube' className={styles.socialIcon} />
          <span className={styles.socialName}> Youtube </span>
        </Button>
      </div>
    )
  }
}
