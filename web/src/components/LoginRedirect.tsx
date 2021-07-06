import { Button, Col, Form, Row, Select } from 'antd';
import React from 'react';
import { authService } from '../services/authService';
import { userService } from '../services/userService';

interface State {
  isNewAccount?: boolean;
}
export class LoginRedirect extends React.Component<any, State> {

  constructor(props: any) {
    super(props);
    this.state = {
      isNewAccount: undefined
    };
  }

  async componentDidMount() {
    const code = new URLSearchParams(window.location.search).get('code') as string;
    const isNewAccount = await authService.zoomLogin(code);
    await userService.initialize();

    if (!isNewAccount) {
      this.loginComplete();
    } else {
      this.setState({ isNewAccount });
    }
  }

  private async onLanguageSelected(values: any): Promise<void> {
    await userService.selectTargetLanguage(values.language);
    this.loginComplete();
  }

  private loginComplete() {
    this.props.history.push("/");
  }

  render() {
    const { isNewAccount } = this.state;
    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        {isNewAccount === undefined &&
          <Col>Login is in progress...</Col>
        }
        {isNewAccount &&
          <Col>
            <Form
              name="basic"
              onFinish={(values) => this.onLanguageSelected(values)}
            >
              <Form.Item
                label="Language"
                name="language"
                rules={[{ required: true, message: 'Please select a language.' }]}
              >
                <Select>
                  <Select.Option value="English">English</Select.Option>
                  <Select.Option value="Russian">Russian</Select.Option>
                </Select>
              </Form.Item>

              <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
                <Button type="primary" htmlType="submit">
                  Save
                </Button>
              </Form.Item>
            </Form>
          </Col>
        }
      </Row>
    );
  }
}