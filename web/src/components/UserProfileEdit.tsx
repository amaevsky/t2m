import { Button, DatePicker, Form, Input, Select } from 'antd';
import { configService } from '../services/configService';
import { User, userService } from '../services/userService';
import country from 'country-list-js';

import moment from 'moment';
import 'moment-timezone';
import { DateFormat } from '../utilities/date';

export const UserProfileEdit = (props: { initialSetup?: boolean, afterSave?: () => void }) => {
  const user = userService.user;
  const save = async (values: any) => {
    const toSave: User = { ...userService.user, ...values, dateOfBirth: moment.utc(values.dateOfBirth).format() };
    await userService.update(toSave, props.initialSetup);

    props.afterSave?.();
  }

  return (
    <Form
      name="basic"
      labelCol={{ span: 7 }}
      wrapperCol={{ span: 17 }}
      initialValues={{ ...user, timezone: moment.tz.guess(), dateOfBirth: user?.dateOfBirth ? moment(user?.dateOfBirth).utc() : null }}
      onFinish={(values) => save(values)}
    >
      <Form.Item
        label="First name"
        name="firstname"
        rules={[
          { required: true, message: 'Please specify firstname.' },
          { max: 50, type: 'string' }
        ]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        label="Last name"
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
        <Select showSearch>
          {configService.config.languages.map(l => <Select.Option value={l}>{l}</Select.Option>)}
        </Select>
      </Form.Item>

      <Form.Item
        label="Language level"
        name="languageLevel"
        rules={[{ required: true, message: 'Please select a language level.' }]}
      >
        <Select>
          {configService.config.languageLevels.map(l => <Select.Option value={l.code}>{l.code} ({l.description})</Select.Option>)}
        </Select>
      </Form.Item>

      <Form.Item
        label="Country of origin"
        name="country"
        rules={[{ required: true, message: 'Please select your country of origin.' }]}
      >
        <Select showSearch>
          {country.names().sort().map(n => <Select.Option value={n}>{n}</Select.Option>)}
        </Select>
      </Form.Item>

      <Form.Item
        label="Date of birth"
        name="dateOfBirth"
        rules={[
          { required: true, message: 'Please select your date of birth.' },
          {
            validator: (_, value: moment.Moment) => {
              return value.clone().add(14, 'year') > moment()
                ? Promise.reject('You have to be at least 14 years old.')
                : Promise.resolve();
            }
          }
        ]}
      >
        <DatePicker format={DateFormat} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        label="Timezone"
        name="timezone"
        rules={[{ required: true, message: 'Please select timezone.' }]}
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

  )
}