// @flow

import React from 'react'
import styles from './styles.scss'
import Pen from './../../../../../../../../model/tools/Pen'
import Eraser from './../../../../../../../../model/tools/Eraser'

import { Grid, Dropdown, Button } from 'semantic-ui-react'

export default class Favorite extends React.Component {
  changeCurrentTool () {
    const fav = this.props.fav
    this.props.changeCurrentTool(fav)
  }
  render () {
    const fav = this.props.fav
    // todo: change the way this is done
    let name = null
    let color = 'black'
    if(fav instanceof Pen){
      name = 'pencil'
      color = fav.color
    }
    else if (fav instanceof Eraser)
      name = 'eraser'
    return (
      <div >
        {/* it could be color:red instead of style:{color:'red'}} but the first one does not support rgba*/}
        <Button circular icon={{name, style:{color}}} className={styles.fav} onClick={this.changeCurrentTool.bind(this)} />
      </div>
    )
  }
}
