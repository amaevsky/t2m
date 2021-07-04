import { Col, Row } from 'antd';
import axios from 'axios';
import React from 'react';

export class LoginRedirect extends React.Component<any, any> {

  async componentDidMount() {
    const code = new URLSearchParams(window.location.search).get('code');
    await axios.get(`https://localhost:44361/api/Auth/login/zoom?authCode=${code}`);
    this.props.history.push("/");
  }

  render() {
    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col>Login is in progress...</Col>
      </Row>
    );
  }
}