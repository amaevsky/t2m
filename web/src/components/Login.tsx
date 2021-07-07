import { Button, Col, Row } from 'antd';
import React from 'react';
import { Redirect } from 'react-router-dom';
import { configService } from '../services/configService';
import { userService } from '../services/userService';

interface State {
  initializing: boolean
}
export class Login extends React.Component<any, State> {

  constructor(props: any) {
    super(props);

    this.state = { initializing: true };
  }

  async componentDidMount() {
    try {
      await userService.initialize();
    } catch {

    }

    if (userService.user) {
      if (userService.user.languageLevel) {
        this.props.history.push("/");
      } else {
        this.props.history.push("/account/setup");
      }
    }

    this.setState({ initializing: false });
  }

  render() {
    const { initializing } = this.state;

    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col>
          {initializing
            ? 'Login is in progress...'
            : <Button type='primary' size='large' onClick={this.redirect}>Login</Button>
          }
        </Col>
      </Row>
    );
  }

  redirect() {
    window.location.href = configService.config.zoomAuthUrl;;
  }
}