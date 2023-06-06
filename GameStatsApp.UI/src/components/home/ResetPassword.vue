<template>
    <div>
        <h2 class="text-center">Reset Password</h2>
        <form @submit.prevent="submitForm">
            <div>
                <ul class="list-group">
                    <li class="list-group-item list-group-item-danger" v-for="errorMessage in errorMessages">{{ errorMessage }}</li>
                </ul>
            </div>
            <div class="mb-3">
                <label for="txtEmail" class="form-label">Email</label>
                <input id="txtEmail" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Email" @blur="v$.form.Email.$touch" aria-describedby="spnEmailErrors">
                <div>
                    <small id="spnEmailErrors" class="form-text text-danger" v-for="error of v$.form.Email.$errors">{{ error.$message }}</small>
                </div>
            </div>
            <div class="row g-2 justify-content-center mb-3">
                <button type="submit" class="btn btn-primary">Send a password reset email</button>
            </div>
            <div>
                <div v-if="loading">
                    <div class="d-flex m-3">
                        <div class="mx-auto">
                            <font-awesome-icon icon="fa-solid fa-spinner" spin size="lg" />
                        </div>
                    </div>
                </div>
                <div v-else-if="showSuccess">
                    <div class="container p-3" style="max-width: 400px;">
                        <div class="mx-auto">
                            <div><span>To Reset your password click the link in the email we just sent you.</span></div>
                            <br />
                            <div class="mx-auto" style="max-width:350px;">
                                <span>If your email has not arrived try these steps:</span>
                                <ul class="pl-4">
                                    <li>Wait 30 mins</li>
                                    <li>Check your spam folder</li>
                                    <li>Try Reset Password again</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
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

    const asyncActiveEmailExists = async (value) => {
        if (value === '') return true;

        return await axios.get('/Home/ActiveEmailExists', { params: { email: value } })
            .then(res => {
                return res.data;
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "ResetPassword",
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    Email: ''
                },
                loading: false,
                showSuccess: false,
                errorMessages: []
            }
        },
        computed: {
        },
        created: function () {
        },
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                this.loading = true;

                axios.post('/Home/ResetPassword', formData)
                    .then((res) => {
                        that.showSuccess = res.data.success;
                        that.errorMessages = res.data.errorMessages;
                        that.loading = false;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }
        },
        validations() {
            return {
                form: {
                    Email: {
                        required: helpers.withMessage('Email is required', required),
                        activeEmailExists: helpers.withMessage('Email not found', withAsync(asyncActiveEmailExists))
                    }
                }
            }
        }
    };
</script>


