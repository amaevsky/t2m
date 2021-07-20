
import React from 'react';
import { Col, Row, Space, Typography } from 'antd';
import { UserProfileEdit } from './UserProfileEdit';

const { Title } = Typography;
export class AccountSetup extends React.Component<any> {

  render() {
    return (
      <Row align='middle' justify='center'>
        <Space size={50} direction='vertical'>
          <div style={{ paddingTop: 50 }}>
            <Title level={4}>Hi! Welcome to <b className='primary-color'>Talk2Me</b></Title>
            <p>To start practicing language, please provide the following details:</p>
          </div>
          <UserProfileEdit afterSave={() => this.props.history.push("/")} />
        </Space>
      </Row >
    );
  }
}