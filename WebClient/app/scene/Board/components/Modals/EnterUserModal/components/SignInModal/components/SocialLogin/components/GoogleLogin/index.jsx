import React from 'react'
import { Icon, Button } from 'semantic-ui-react'

export default class SocialLogin extends React.Component {
  render () {
    return (
      <Button circular color='google plus' style={this.props.style} className={this.props.className}>
        <Icon name='google plus' className={this.props.iconClassName} />
        <span className={this.props.nameClassName}> Google Plus </span>
      </Button>
    )
  }
}
