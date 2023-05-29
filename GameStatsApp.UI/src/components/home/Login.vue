<template>
    <div>
        <h2>Welcome to GameStatsApp</h2>
        <form @submit.prevent="submitForm">
            <div>
                <ul class="list-group">
                    <li class="list-group-item list-group-item-danger" v-for="errorMessage in errorMessages">{{ errorMessage }}</li>
                </ul>
            </div>
            <div class="mb-1">
                <label for="txtUserName" class="form-label">Username</label>
                <input id="txtUserName" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Username" @blur="v$.form.Username.$touch" aria-describedby="spnUserNameErrors">
                <div>
                    <span id="spnUserNameErrors" class="form-text text-danger" v-for="error of v$.form.Username.$errors">{{ error.$message }}</span>
                </div>
            </div>
            <div class="mb-3">
                <label for="txtPassword" class="form-label">Password</label>
                <input id="txtPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.Password" @blur="v$.form.Password.$touch" aria-describedby="spnPasswordErrors">
                <div>
                    <span id="spnPasswordErrors" class="form-text text-danger" v-for="error of v$.form.Password.$errors">{{ error.$message }}</span>
                </div>
            </div>   
            <div class="row g-2 justify-content-center">
                <button type="submit" class="btn btn-primary">Log In</button>
                <div class="text-center"><small class="fw-bold">OR</small></div>
                <button type="submit" class="btn btn-primary">Continue with Facebook</button>
                <button type="submit" class="btn btn-primary">Continue with Google</button>
            </div>
        </form>
    </div>
</template>
<script>
    import { getFormData } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers } from '@vuelidate/validators';
    const { withAsync } = helpers;

    const asyncActiveUsernameExists = async (value) => {
        if (value === '') return true;

        return await axios.get('/Home/ActiveUsernameExists', { params: { username: value } })
            .then(res => {
                return res.data;
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "Login",
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    Username: '',
                    Password: ''
                },
                errorMessages: []
            }
        },
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);

                axios.post('/Home/Login', formData)
                    .then((res) => {
                        if (res.data.success) {
                            location.reload();
                        } else {
                            that.errorMessages = res.data.errorMessages;
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }
        },
        validations() {
            return {
                form: {
                    Username: {
                        required: helpers.withMessage('Username is required', required),
                        activeUsernameExists: helpers.withMessage('Invalid Username', withAsync(asyncActiveUsernameExists))
                    },
                    Password: {
                        required: helpers.withMessage('Password is required', required)
                    }
                }
            }
        }
    };
</script>


