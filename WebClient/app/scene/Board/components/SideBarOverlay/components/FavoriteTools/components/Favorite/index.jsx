// @flow

import React from 'react'
import styles from './styles.scss'

import { Grid, Dropdown, Button } from 'semantic-ui-react'

export default class Favorite extends React.Component {
  render () {
    return (
      <div >
        {/*it could be color:red instead of style:{color:'red'}} but the first one does not support rgba*/}
        <Button circular icon={{name:'pencil', style:{color:'red'}}} className={styles.fav} >

        </Button>
      </div>
    )
  }
}
