import { Button, Col, Form, Input, Row, Select } from 'antd';
import React from 'react';
import { configService } from '../services/configService';
import { userService } from '../services/userService';

import moment from 'moment';
import 'moment-timezone';

interface State {

}

export class AccountSetup extends React.Component<any, State> {

  constructor(props: any) {
    super(props);

    this.state = {};
  }

  render() {
    const user = { ...userService.user };
    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col span={10}>
          <Form
            name="basic"
            labelCol={{ span: 5 }}
            wrapperCol={{ span: 19 }}
            initialValues={{ ...user, timezone: moment.tz.guess() }}
            onFinish={(values) => this.save(values)}
          >
            <Form.Item
              label="Firstname"
              name="firstname"
              rules={[
                { required: true, message: 'Please specify firstname.' },
                { max: 50, type: 'string' }
              ]}
            >
              <Input />
            </Form.Item>

            <Form.Item
              label="Lastname"
              name="lastname"
              rules={[
                { required: true, message: 'Please specify Lastname.' },
                { max: 50, type: 'string' }
              ]}
            >
              <Input />
            </Form.Item>

            <Form.Item
              label="Email"
              name="email"
              rules={[{ required: true, message: 'Please specify email.' }]}
            >
              <Input type='email' disabled />
            </Form.Item>

            <Form.Item
              label="Target language"
              name="targetLanguage"
              rules={[{ required: true, message: 'Please select a target language.' }]}
            >
              <Select>
                {configService.config.languages.map(l => <Select.Option value={l}>{l}</Select.Option>)}
              </Select>
            </Form.Item>

            <Form.Item
              label="Language level"
              name="languageLevel"
              rules={[{ required: true, message: 'Please select a language level.' }]}
            >
              <Select>
                {configService.config.languageLelels.map(l => <Select.Option value={l.code}>{l.code}({l.description})</Select.Option>)}
              </Select>
            </Form.Item>

            <Form.Item
              label="Timezone"
              name="timezone"
              rules={[{ required: true, message: 'Please select a timezone.' }]}
            >
              <Select>
                {
                  moment.tz.names().map(t => <Select.Option value={t}>{t}</Select.Option>)
                }
              </Select>
            </Form.Item>

            <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
              <Button type="primary" htmlType="submit">
                Save
              </Button>
            </Form.Item>
          </Form >
        </Col>
      </Row >
    );
  }

  async save(values: any) {
    const toSave = { ...userService.user, ...values };
    await userService.update(toSave);
    this.props.history.push("/");
  }
}