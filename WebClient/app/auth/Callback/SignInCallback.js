import React from 'react'
import {
  Loader
} from 'semantic-ui-react'

class Callback extends React.Component {
  handleAuthentication = (props) => {
    if (/access_token|id_token|error/.test(props.location.hash)) {
      this.props.auth.handleAuthentication(props)
    }
  }

  componentWillUpdate () {
    this.handleAuthentication(this.props)
  }

  render () {
    return (
      <div>
        <Loader active content='Signing You In ...' />
      </div>
    )
  }
}

export default Callback
