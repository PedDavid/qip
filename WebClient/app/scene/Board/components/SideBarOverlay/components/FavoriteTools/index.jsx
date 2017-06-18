// @flow

import React from 'react'
import Favorite from './components/Favorite'
import styles from './styles.scss'

import { Grid, Button } from 'semantic-ui-react'

export default class FavoriteTools extends React.Component {
  render () {
    return (
      <div className={styles.quickBtnsContainer}>
        <Grid divided textAlign='center'>
            <Grid.Row columns={1} className={styles.rows} style={{padding:'7px'}}> {/*todo: (simaovii) I don't understand why padding in className is not working...*/}
              <Grid.Column>
                <Button circular icon='list layout' onClick={this.props.toggleSideBar.bind(this)}>
                  
                </Button>
              </Grid.Column>
            </Grid.Row>
            {this.props.favorites.map(favorite => (
              <Grid.Row columns={1} className={styles.rows} style={{padding:'7px'}}>
                <Grid.Column>
                  <Favorite fav={favorite}/>
                </Grid.Column>
              </Grid.Row>
            ))}
            <Grid.Row columns={1} className={styles.rows} style={{padding:'7px'}}>
              <Grid.Column>
                <Button circular icon='plus' >
                  
                </Button>
              </Grid.Column>
            </Grid.Row>
        </Grid>
      </div>
    )
  }
}
