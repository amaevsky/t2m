import { Col, Row } from 'antd';
import React from 'react';
import { authService } from '../services/authService';
import { userService } from '../services/userService';

export class LoginRedirect extends React.Component<any> {

  async componentDidMount() {
    const code = new URLSearchParams(window.location.search).get('code') as string;
    await authService.zoomLogin(code);
    this.props.history.push("/login/");
  }

  render() {
    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col>Login is in progress...</Col>
      </Row>
    );
  }
}