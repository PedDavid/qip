import auth0 from 'auth0-js'
import { AUTH_CONFIG } from './AuthVariables'

export default class Auth {
  auth0 = new auth0.WebAuth({
    domain: AUTH_CONFIG.domain,
    clientID: AUTH_CONFIG.clientId,
    redirectUri: AUTH_CONFIG.callbackUrl,
    audience: `https://${AUTH_CONFIG.domain}/userinfo`,
    responseType: 'token id_token',
    scope: 'openid profile'
  })

  constructor () {
    this.login = this.login.bind(this)
    this.logout = this.logout.bind(this)
    this.handleAuthentication = this.handleAuthentication.bind(this)
    this.isAuthenticated = this.isAuthenticated.bind(this)
    this.getAccessToken = this.getAccessToken.bind(this)
    this.getProfile = this.getProfileAsync.bind(this)
    this.userProfile = null
  }

  login () {
    this.auth0.authorize()
  }

  handleAuthentication (props) {
    this.auth0.parseHash((err, authResult) => {
      if (authResult && authResult.accessToken && authResult.idToken) {
        this.setSession(authResult)
        this.getProfileAsync((err, profile) => {
          if (err) throw new Error(err)
          console.log(profile)
          window.localStorage.setItem('profile', JSON.stringify(profile))
          props.history.replace('/')
        })
      } else if (err) {
        props.history.replace('/')
        console.log(err)
        window.alert(`Error: ${err.error}. Check the console for further details.`)
      }
    })
  }

  setSession (authResult) {
    // Set the time that the access token will expire at
    let expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime())
    window.localStorage.setItem('access_token', authResult.accessToken)
    window.localStorage.setItem('id_token', authResult.idToken)
    window.localStorage.setItem('expires_at', expiresAt)
    // navigate to the home route
    // history.replace('/home')
  }

  getAccessToken () {
    const accessToken = window.localStorage.getItem('access_token')
    if (!accessToken) {
      throw new Error('No access token found')
    }
    return accessToken
  }

  getProfileAsync (cb) {
    let accessToken = this.getAccessToken()
    this.auth0.client.userInfo(accessToken, (err, profile) => {
      if (profile) {
        this.userProfile = profile
      }
      cb && cb(err, profile)
    })
  }

  // todo: this might be called when there is no profile yet. correct this and change all calls to this method
  tryGetProfile () {
    // if it was already created and user authentication is valid, return profile
    if (this.isAuthenticated() && window.localStorage.getItem('profile') != null) {
      return JSON.parse(window.localStorage.getItem('profile'))
    }
  }

  logout () {
    // Clear access token and ID token from local storage
    window.localStorage.removeItem('access_token')
    window.localStorage.removeItem('id_token')
    window.localStorage.removeItem('expires_at')
    window.localStorage.removeItem('profile')
    this.userProfile = null
    // navigate to the home route
    // history.replace('/home')
  }

  isAuthenticated () {
    // Check whether the current time is past the
    // access token's expiry time
    const expiresAt = JSON.parse(window.localStorage.getItem('expires_at'))
    return new Date().getTime() < expiresAt
  }
}
