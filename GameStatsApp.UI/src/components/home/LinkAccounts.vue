<template>
    <div class="mx-auto">
        <h2 class="text-center mb-1">Welcome to GameStatsApp</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div class="mb-3">
                <change-username :username="indexvm.username"></change-username>
            </div>
            <div class="text-center">
                <p class="lead text-dark">Get started by linking an account</p>
            </div>
            <div class="row g-2 justify-content-center">
                <button type="submit" class="btn btn-outline-dark d-flex"><font-awesome-icon icon="fa-brands fa-steam" size="xl" style="color: #0a3169;" /><span class="mx-auto">Link Steam account</span></button>
                <button type="button" class="btn btn-outline-dark d-flex" @click="onXboxClick"><font-awesome-icon icon="fa-brands fa-xbox" size="xl" style="color: #107711;" /><span class="mx-auto">Link Xbox account</span></button>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        name: "LinkAccounts",
        props: {
            indexvm: Object
        },
        data() {
            return {
                form: {
                    Username: this.username
                },
                errorMessages: [],
                successToast: {},
                showSignUpModal: false,
                editUsername: false
            }
        },
        computed: {
        },
        mounted: function () {
        },
        methods: {   
            onXboxClick() {
                location.href = this.indexvm.windowsLiveAuthUrl;
            }    
        }
    };
</script>


