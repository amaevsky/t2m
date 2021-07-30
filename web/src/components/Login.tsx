import { Button, Col, Row, Spin, Typography } from 'antd';
import React from 'react';
import { configService } from '../services/configService';
import { userService } from '../services/userService';
import { routes } from './App';

const { Title } = Typography;

interface State {
  initializing: boolean
}
export class Login extends React.Component<any, State> {

  constructor(props: any) {
    super(props);

    this.state = { initializing: true };
  }

  async componentDidMount() {
    if (userService.isAuthenticated) {
      if (userService.isAccountReady) {
        const { from } = this.props.location.state || { from: { pathname: routes.default } };
        this.props.history.push(from);
      } else {
        this.props.history.push(routes.login.accountSetup);
      }
    }

    this.setState({ initializing: false });
  }

  render() {
    const { initializing } = this.state;

    return (
      <Row style={{ flex: 1 }} align='middle' justify='center'>
        <Col>
          {initializing
            ? <Spin size='large'></Spin>
            :
            <>
              <Title level={2}>Welcome to <b className='primary-color'>Talk2Me</b></Title>
              <p>In order to use our app you have to login via Zoom</p>
              <Row justify='center' style={{ paddingTop: 16 }}>
                <Button style={{ width: '100%' }} type='primary' size='large' onClick={this.redirect}>Login via Zoom</Button>
              </Row>
            </>
          }
        </Col>
      </Row>
    );
  }

  redirect() {
    window.location.href = configService.config.zoomAuthUrl;;
  }
}