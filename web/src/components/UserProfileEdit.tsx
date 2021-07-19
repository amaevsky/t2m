import { Button, Form, Input, Select } from 'antd';
import { configService } from '../services/configService';
import { userService } from '../services/userService';

import moment from 'moment';

export const UserProfileEdit = (props: { afterSave?: () => void }) => {
  const user = userService.user;
  const save = async (values: any) => {
    const toSave = { ...userService.user, ...values };
    await userService.update(toSave);

    props.afterSave?.();
  }

  return (
    <Form
      name="basic"
      labelCol={{ span: 5 }}
      wrapperCol={{ span: 19 }}
      initialValues={{ ...user }}
      onFinish={(values) => save(values)}
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
          {configService.config.languageLevels.map(l => <Select.Option value={l.code}>{l.code}({l.description})</Select.Option>)}
        </Select>
      </Form.Item>

      <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
        <Button type="primary" htmlType="submit">
          Save
        </Button>
      </Form.Item>
    </Form >

  )
}