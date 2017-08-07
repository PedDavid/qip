import React from 'react'
import { Icon, Button } from 'semantic-ui-react'

export default function SocialLogin (props) {
  return (
    <Button circular color='google plus' style={props.style} className={props.className}>
      <Icon name='google plus' className={props.iconClassName} />
      <span className={props.nameClassName}> Google Plus </span>
    </Button>
  )
}
