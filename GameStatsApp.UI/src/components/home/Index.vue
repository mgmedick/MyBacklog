<template>
    <div>
        <div v-if="isauth">          
        </div>
        <div v-else>
            <login @forgotpass="showResetModal = !(showLoginModal = false)" @signup="showSignUpModal = !(showLoginModal = false)"/>
        </div>   
        <modal v-if="showResetModal" contentclass="cmv-modal-md" @close="showResetModal = false">
            <template v-slot:title>
                Reset Password
            </template>
            <div class="container">
                <reset-password />
            </div>
        </modal>
        <modal v-if="showSignUpModal" contentclass="cmv-modal-md" @close="showSignUpModal = false">
            <template v-slot:title>
                Sign Up
            </template>
            <div class="container">
                <signup />
            </div>
        </modal>           
    </div>   
</template>
<script>
    export default {
        name: "Index",
        props: {
            isauth: Boolean
        },
        data: function () {
            return {
                showResetModal: false,
                showSignUpModal: false
            }
        },
        computed: {
        },
        watch: {
        },
        created: function () {
        },        
        methods: {
        }
    };
</script>






